using Anno.EngineData;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anno.Plugs.FurionService
{
    public class ProductModule : BaseModule
    {
        private readonly ISqlSugarRepository sqlSugarRepository;
        public ProductModule(ISqlSugarRepository sqlSugarRepository) {
            this.sqlSugarRepository = sqlSugarRepository;
        }
        public Task<string> SayHi(string name) {
            return Task.FromResult($"{name} 你好，我是Anno。");
        }
    }
}
