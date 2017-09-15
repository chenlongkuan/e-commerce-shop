using System;
using System.Linq;
using System.Linq.Expressions;
using Msg.Entities;
using Msg.Providers.Repository;
using Msg.Tools;
using Msg.Tools.ExpressionExtend;
using Msg.Tools.Logging;

namespace Msg.Bll.Helpers
{
   public  class SupliersHelper
    {
       private readonly EfRepository<SuppliersEntity, int> _supplierRepository = EfRepository<SuppliersEntity, int>.Instance;




       public IQueryable<SuppliersEntity> GetAllSupliersEntities(string linkMan,string linkTel,string userName,string protectName,out int total,int page ,int size = 10)
       {
           Expression<Func<SuppliersEntity, bool>> filter = f => f.Id > 0;
           Expression<Func<SuppliersEntity, DateTime>> order = f => f.CreateTime;

           if (!string.IsNullOrEmpty(linkMan))
           {
               filter = filter.And(f => f.LinkMan.Contains(linkMan));
           }
           if (!string.IsNullOrEmpty(linkTel))
           {
               filter = filter.And(f => f.LinkTel.Contains(linkTel));
           }
           if (!string.IsNullOrEmpty(userName))
           {
               filter = filter.And(f => f.User.UserName.Contains(userName));
           }
           if (!string.IsNullOrEmpty(protectName))
           {
               filter = filter.And(f => f.ProductsApply.Any(p => p.Product.Name.Contains(protectName)));
           }
           return _supplierRepository.LoadEntitiesByPaging(page, 10, filter, order, OrderingOrders.DESC, out total);
       }


       public SuppliersEntity GetEntityById(int id)
       {
           if (id <= 0)
           {
               return new SuppliersEntity();
           }
           else
           {
               return _supplierRepository.FindById(id);
           }
       }


       public OperationResult DealSupplierApply(int id,bool type,string refusedReason)
       {
          var model =  _supplierRepository.FindById(id);
           model.IsVerified = type;
           model.RefusedReason = refusedReason;
           try
           {
               _supplierRepository.UpdateEntity(model);
               return  new OperationResult(OperationResultType.Success,"操作成功！");

           }
           catch (Exception ex)
           {
               LogHelper.WriteException("DealSupplierApply => "+ex.Message,ex);
               return new OperationResult(OperationResultType.NoChanged,"失败！");
           }
       
       }
    }
}
