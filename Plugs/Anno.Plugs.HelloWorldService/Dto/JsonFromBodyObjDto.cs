using System;
using System.Collections.Generic;
using System.Text;

namespace Anno.Plugs.HelloWorldService.Dto
{
    public class JsonFromBodyObjDto
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 年龄
        /// </summary>
        public int  Age { get; set; }
        /// <summary>
        /// 体重
        /// </summary>
        public decimal Weight { get; set; }

        /// <summary>
        /// 爱好
        /// </summary>
        public List<string> Hobby { get; set; }
    }
}
