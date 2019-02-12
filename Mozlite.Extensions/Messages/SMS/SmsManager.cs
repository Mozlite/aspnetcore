using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mozlite.Data;

namespace Mozlite.Extensions.Messages.SMS
{
    /// <summary>
    /// 短信管理类型。
    /// </summary>
    public class SmsManager : ObjectManager<Note>, ISmsManager
    {
        private readonly ConcurrentDictionary<string, ISmsClient> _clients;

        /// <summary>
        /// 初始化类<see cref="SmsManager"/>。
        /// </summary>
        /// <param name="context">数据库操作实例。</param>
        /// <param name="clients">客户端列表。</param>
        public SmsManager(IDbContext<Note> context, IEnumerable<ISmsClient> clients) : base(context)
        {
            _clients = new ConcurrentDictionary<string, ISmsClient>(clients.ToDictionary(x => x.Name), StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 保存短信。
        /// </summary>
        /// <param name="client">客户端发送器名称<see cref="ISmsClient.Name"/>。</param>
        /// <param name="phoneNumbers">电话号码，多个电话号码使用“,”分开。</param>
        /// <param name="message">消息字符串。</param>
        /// <returns>返回保存结果。</returns>
        public virtual DataResult Save(string client, string phoneNumbers, string message)
        {
            if (!_clients.TryGetValue(client, out var smsClient))
                return "短信发送客户端不存在！";
            var note = new Note();
            note.Client = client;
            note.Message = message;
            var numbers = phoneNumbers
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Distinct()
                .ToList();
            return DataResult.FromResult(Context.BeginTransaction(db =>
            {
                foreach (var number in numbers)
                {
                    note.Id = 0;
                    note.PhoneNumber = number;
                    var prev = db.AsQueryable()
                        .Where(x => x.HashKey == note.HashKey)
                        .OrderByDescending(x => x.CreatedDate)
                        .FirstOrDefault();
                    if (smsClient.IsDuplicated(note, prev))
                        continue;
                    db.Create(note);
                }

                return true;
            }), DataAction.Created);
        }

        /// <summary>
        /// 保存短信。
        /// </summary>
        /// <param name="client">客户端发送器名称<see cref="ISmsClient.Name"/>。</param>
        /// <param name="phoneNumbers">电话号码，多个电话号码使用“,”分开。</param>
        /// <param name="message">消息字符串。</param>
        /// <returns>返回保存结果。</returns>
        public virtual async Task<DataResult> SaveAsync(string client, string phoneNumbers, string message)
        {
            if (!_clients.TryGetValue(client, out var smsClient))
                return "短信发送客户端不存在！";
            var note = new Note();
            note.Client = client;
            note.Message = message;
            var numbers = phoneNumbers
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Distinct()
                .ToList();
            return DataResult.FromResult(await Context.BeginTransactionAsync(async db =>
            {
                foreach (var number in numbers)
                {
                    note.Id = 0;
                    note.PhoneNumber = number;
                    var prev = await db.AsQueryable()
                        .Where(x => x.HashKey == note.HashKey)
                        .OrderByDescending(x => x.CreatedDate)
                        .FirstOrDefaultAsync();
                    if (smsClient.IsDuplicated(note, prev))
                        continue;
                    await db.CreateAsync(note);
                }

                return true;
            }), DataAction.Created);
        }

        /// <summary>
        /// 发送并保存短信。
        /// </summary>
        /// <param name="note">短信实例对象。</param>
        /// <returns>返回发送结果。</returns>
        public virtual async Task<SmsResult> SendAsync(Note note)
        {
            if (!_clients.TryGetValue(note.Client, out var client))
                return false;
            var result = await client.SendAsync(note);
            if (result.Status == NoteStatus.Failured)
            {
                note.TryTimes++;
                if (note.TryTimes >= SmsSettings.MaxTimes)
                    note.Status = NoteStatus.Failured;

                note.Msg = result.Msg;
                await SaveAsync(note);
                return false;
            }

            note.Status = NoteStatus.Completed;
            note.SentDate = DateTimeOffset.Now;
            note.TryTimes = 0;
            await SaveAsync(note);
            return true;
        }

        /// <summary>
        /// 发送并保存短信。
        /// </summary>
        /// <param name="client">客户端发送器名称<see cref="ISmsClient.Name"/>。</param>
        /// <param name="phoneNumber">电话号码。</param>
        /// <param name="message">消息字符串。</param>
        /// <returns>返回发送结果。</returns>
        public Task<SmsResult> SendAsync(string client, string phoneNumber, string message)
        {
            var note = new Note();
            note.Client = client;
            note.PhoneNumber = phoneNumber;
            note.Message = message;
            return SendAsync(note);
        }

        /// <summary>
        /// 加载未发送的短信列表。
        /// </summary>
        /// <returns>未发送的短信列表。</returns>
        public virtual Task<IEnumerable<Note>> LoadAsync()
        {
            return Context.AsQueryable()
                .WithNolock()
                .OrderBy(x => x.Id)
                .Where(x => x.Status == NoteStatus.Pending)
                .AsEnumerableAsync(SmsSettings.BatchSize);
        }
    }
}