using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTool.Parsers
{
    internal class TemplateModel
    {

        internal static List<string> ValidModelName = new List<string>
        {
            "index",
            "yyyy",
            "mm",
            "dd"
        };

        //public string TemplateName { get; set; } = string.Empty;

        public string TemplateString { get; set; } = string.Empty;


        #region
        /// <summary>
        /// 如果是Index 模板，
        /// TemplatePart1=Index
        /// TemplatePart2=共有多少位数字
        /// TemplatePart3=从多少开始
        /// </summary>

        /// <summary>
        /// 如果是Index 模板，
        /// TemplatePart1=Index
        /// TemplatePart2=共有多少位数字
        /// TemplatePart3=从多少开始
        /// </summary>
        public string TemplatePart1 { get; set; } = string.Empty;
        /// <summary>
        /// 如果是Index 模板，
        /// TemplatePart1=Index
        /// TemplatePart2=共有多少位数字
        /// TemplatePart3=从多少开始
        /// </summary>
        public string TemplatePart2 { get; set; } = string.Empty;
        /// <summary>
        /// 如果是Index 模板，
        /// TemplatePart1=Index
        /// TemplatePart2=共有多少位数字
        /// TemplatePart3=从多少开始
        /// </summary>
        public string TemplatePart3 { get; set; } = string.Empty;


        public int IntTemplatePart2 { get; set; } 

        public int IntTemplatePart3 { get; set; } 


        #endregion

    }
}
