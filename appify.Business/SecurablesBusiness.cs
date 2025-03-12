using appify.Business.Contract;
using appify.DataAccess.Contract;
using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.Business
{
    public class SecurablesBusiness : ISecurablesBusiness
    {
        private ISecurablesRepository repository;

        public SecurablesBusiness(ISecurablesRepository repository)
        {
            this.repository = repository;
        }
<<<<<<< HEAD
        public bool Delete(Int32 SecurablesID)
        {
            return repository.Delete(SecurablesID);
        }

        public Securables Get(Int32 SecurablesID)
        {
            return repository.Get(SecurablesID);
        }

        public Int64 GetSecurablesCount()
        {
            return repository.GetSecurablesCount();
        }

        public List<Securables> ListAll()
        {
            return repository.ListAll();
        }
        public Securables Save(Securables item)
        {
            return repository.Save(item);
        }


        #region Securable Functions (OBSOLETE)

       
        //public bool SFDelete(Int32 FunctionID)
        //{
        //    return repository.SFDelete(FunctionID);
        //}

        //public SecurablesFunction SFGet(Int32 FunctionID)
        //{
        //    return repository.SFGet(FunctionID);
        //}

        //public Int64 SFGetSecurablesCount()
        //{
        //    return repository.SFGetSecurablesCount();
        //}
        //public List<SecurablesFunction> SFList(Int32 SecurablesID)
        //{
        //    return repository.SFList(SecurablesID);
        //}
        //public List<SecurablesFunction> SFListAll()
        //{
        //    return repository.SFListAll();
        //}
        //public SecurablesFunction SFSave(SecurablesFunction item)
        //{
        //    return repository.SFSave(item);
        //}

        #endregion
=======
    public bool Delete(Int32 SecurablesID)
    {
        return repository.Delete(SecurablesID);
    }

        public Securables Get(Int32 SecurablesID)
    {
        return repository.Get(SecurablesID);
    }

    public Int64 GetSecurablesCount()
    {
        return repository.GetSecurablesCount();
    }

    public List<Securables> ListAll()
    {
        return repository.ListAll();
    }
    public Securables Save(Securables item)
    {
        return repository.Save(item);
    }

        public bool SFDelete(Int32 FunctionID)
        {
            return repository.SFDelete(FunctionID);
        }

        public SecurablesFunction SFGet(Int32 FunctionID)
        {
            return repository.SFGet(FunctionID);
        }

        public Int64 SFGetSecurablesCount()
        {
            return repository.SFGetSecurablesCount();
        }
        public List<SecurablesFunction> SFList(Int32 SecurablesID)
        {
            return repository.SFList(SecurablesID);
        }
        public List<SecurablesFunction> SFListAll()
        {
            return repository.SFListAll();
        }
        public SecurablesFunction SFSave(SecurablesFunction item)
        {
            return repository.SFSave(item);
        }
>>>>>>> origin/main

    }
}
