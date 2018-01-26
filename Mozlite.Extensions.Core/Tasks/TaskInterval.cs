using System;
using System.Linq;
using Microsoft.AspNetCore.Html;
using Mozlite.Data;

namespace Mozlite.Extensions.Tasks
{
    /// <summary>
    /// 任务时间间隔类型。
    /// </summary>
    public class TaskInterval
    {
        private readonly int _month;
        private readonly int _interval;
        private readonly int _day;
        private readonly TimeSpan _time;
        private readonly TaskMode _mode;

        private TaskInterval(int interval = 0, int month = 0, int day = 0, TimeSpan? time = null)
        {
            if (interval > 0)
            {
                _mode = TaskMode.Interval;
                _interval = interval;
            }
            else if (month > 0)
            {
                _mode = TaskMode.Month;
                _month = month;
                _day = day;
                _time = time ?? TimeSpan.Zero;
            }
            else if (day > 0)
            {
                _mode = TaskMode.Day;
                _day = day;
                _time = time ?? TimeSpan.Zero;
            }
            else
            {
                _mode = TaskMode.Hour;
                _time = time ?? TimeSpan.Zero;
            }
        }

        /// <summary>
        /// 返回表示当前对象的字符串。
        /// </summary>
        /// <returns>
        /// 表示当前对象的字符串。
        /// </returns>
        public override string ToString()
        {
            switch (_mode)
            {
                case TaskMode.Month:
                    return string.Concat(_month.ToString("D2"), "-", _day.ToString("D2"), " ", _time.ToString());
                case TaskMode.Day:
                    return string.Concat(_day.ToString("D2"), " ", _time.ToString());
                case TaskMode.Hour:
                    return _time.ToString();
            }
            return _interval.ToString();
        }

        public IHtmlContent ToHtmlString()
        {
            switch (_mode)
            {
                case TaskMode.Month:
                    return new HtmlString(string.Concat("每年",_month.ToString("D2"), "月", _day.ToString("D2"), "日 ", _time.ToString()));
                case TaskMode.Day:
                    return new HtmlString(string.Concat("每月",_day.ToString("D2"), "日 ", _time.ToString()));
                case TaskMode.Hour:
                    return new HtmlString($"每天{_time}");
            }
            return new HtmlString($"每隔{_interval}秒");
        }

        /// <summary>
        /// 时间模式。
        /// </summary>
        private enum TaskMode
        {
            /// <summary>
            /// 每隔多少秒执行一次。
            /// </summary>
            Interval,
            /// <summary>
            /// 每天几点几分执行一次。
            /// </summary>
            Hour,
            /// <summary>
            /// 每月几日几点几分执行。
            /// </summary>
            Day,
            /// <summary>
            /// 每年几月几日几点几分执行。
            /// </summary>
            Month,
        }

        /// <summary>
        /// 隐式转换<see cref="TaskInterval"/>。
        /// </summary>
        /// <param name="interval">每隔几秒钟执行一次。</param>

        public static implicit operator TaskInterval(int interval)
        {
            return new TaskInterval(interval);
        }

        /// <summary>
        /// 隐式转换<see cref="TaskInterval"/>。
        /// </summary>
        /// <param name="time">每天几点几分执行一次。</param>

        public static implicit operator TaskInterval(TimeSpan time)
        {
            return new TaskInterval(time: time);
        }

        /// <summary>
        /// 隐式转换<see cref="TaskInterval"/>。
        /// </summary>
        /// <param name="date">每年几月几日几点几分执行。</param>

        public static implicit operator TaskInterval(DateTime date)
        {
            return new TaskInterval(month: date.Month, day: date.Day, time: date.TimeOfDay);
        }

        /// <summary>
        /// 隐式转换<see cref="TaskInterval"/>。
        /// </summary>
        /// <param name="date">每年几月几日几点几分执行字符串表达式。</param>

        public static implicit operator TaskInterval(string date)
        {
            Check.NotEmpty(date, nameof(date));
            date = date.Trim();
            
            if (int.TryParse(date, out var interval))
                return new TaskInterval(interval);

            var dateTimes = date.Split(' ').Select(d => d.Trim()).ToArray();
            if (dateTimes.Length == 1)
                return new TaskInterval(time: TimeSpan.Parse(date));

            if (dateTimes.Length != 2)
                throw new Exception("格式错误，必须为MM-dd HH:mm格式。");

            var dates = dateTimes[0].Split('-').Select(d => d.Trim()).ToArray();
            if (dates.Length == 1)
                return new TaskInterval(day: Convert.ToInt32(dates[0]), time: TimeSpan.Parse(dateTimes[1]));
            return new TaskInterval(month: Convert.ToInt32(dates[dates.Length - 2]), day: Convert.ToInt32(dates[dates.Length - 1]), time: TimeSpan.Parse(dateTimes[1]));
        }

        /// <summary>
        /// 下一次运行时间。
        /// </summary>
        /// <returns>返回下一次运行时间。</returns>
        public DateTime Next()
        {
            var now = DateTime.Now;
            switch (_mode)
            {
                case TaskMode.Month:
                    return new DateTime(now.Year, _month, _day).Add(_time).AddYears(1);
                case TaskMode.Day:
                    return new DateTime(now.Year, now.Month, _day).Add(_time).AddMonths(1);
                case TaskMode.Hour:
                    return now.Date.Add(_time).AddDays(1);
            }

            return now.AddSeconds(_interval);
        }
    }
}