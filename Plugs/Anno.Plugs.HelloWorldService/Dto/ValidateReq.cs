/****************************************************** 
Writer: Du YanMing-admin
Mail:dym880@163.com
Create Date: 7/2/2021 4:49:32 PM
Functional description： ValidateReq
******************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;


namespace Anno.Plugs.HelloWorldService.Dto
{
    public class ValidateReq
    {
        public List<string> permissions { get; set; }
        [Required(ErrorMessage ="缺少token参数")]        
        public string token { get; set; }
    }
}
