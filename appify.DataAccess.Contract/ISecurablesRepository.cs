using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.DataAccess.Contract
{
    public interface ISecurablesRepository
    {
        public Securables Save(Securables item);
        public bool Delete(Int32 SecurablesID);

        public Securables Get(Int32 SecurablesID);

        public List<Securables> ListAll();

        public Int64 GetSecurablesCount();


        //public SecurablesFunction SFSave(SecurablesFunction item);
        //public bool SFDelete(Int32 FunctionID);

        //public SecurablesFunction SFGet(Int32 FunctionID);
        //public List<SecurablesFunction> SFList(Int32 FunctionID);
        //public List<SecurablesFunction> SFListAll();

        //public Int64 SFGetSecurablesCount();


    }
}
