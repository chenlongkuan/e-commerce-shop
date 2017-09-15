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
   public  class CreditGoodsHelper
    {
        private readonly EfRepository<CreditGoodsEntity, int> _cgoodsRepository =
          EfRepository<CreditGoodsEntity, int>.Instance;

        /// <summary>
        /// Gets the credit goods entities.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="date">The date.</param>
        /// <param name="total">The total.</param>
        /// <param name="isVirtual">The is virtual.</param>
        /// <param name="cgoods">The cgoods.</param>
        /// <returns></returns>
        public IQueryable<CreditGoodsEntity> GetCreditGoodsEntities(int page,DateTime? date,out int total,bool? isVirtual, string cgoods = "")
        {
            Expression<Func<CreditGoodsEntity, bool>> filter = f => f.IsUseable;

            if (date != null)
            {

                filter = filter.And(f => f.StartTime <= date && f.EndTime >= date);
            }
            if (isVirtual != null)

            {
                
                filter = filter.And(f => f.IsVirtual == isVirtual);
            }
            if (cgoods != "")
            {
                filter = filter.And(f => f.Name.Contains(cgoods));
            }

           return  _cgoodsRepository.LoadEntitiesByPaging(page, 10, filter, f => f.CreateTime, OrderingOrders.DESC, out total);

        }


       public OperationResult DealCreaditGoodsEdit(CreditGoodsEntity cgoodEntity,string type)
       {
           var result = new OperationResult(OperationResultType.NoChanged);
           switch (type)
           {
               case "Add":
                   
                   result = VaildPara(cgoodEntity);
                   if (result.ResultType.Equals(OperationResultType.Success))
                   {
                       cgoodEntity.IsUseable = true;
                       _cgoodsRepository.AddEntity(cgoodEntity);
                   }
                   break;
               case "Modify":
                   result = VaildPara(cgoodEntity);
                   if (result.ResultType.Equals(OperationResultType.Success))
                   {
                       var modifyentity =  _cgoodsRepository.FindById(cgoodEntity.Id);
                       modifyentity.Desc = cgoodEntity.Desc;
                       modifyentity.EndTime = cgoodEntity.EndTime;
                       modifyentity.ExchangeTimes = cgoodEntity.ExchangeTimes;
                       modifyentity.IsVirtual = cgoodEntity.IsVirtual;
                       modifyentity.Logo = cgoodEntity.Logo;
                       modifyentity.Name = cgoodEntity.Name;
                       modifyentity.NeedCredits = cgoodEntity.NeedCredits;
                       modifyentity.Quantity = cgoodEntity.Quantity;
                       modifyentity.Spec = cgoodEntity.Spec;
                       modifyentity.StartTime = cgoodEntity.StartTime;
                       modifyentity.IsUseable = true;
                       _cgoodsRepository.UpdateEntity(modifyentity);
                   }
                   break;
               case "Del":
                   var modifyentity1 = _cgoodsRepository.FindById(cgoodEntity.Id);
                   modifyentity1.IsUseable = false;
                   if (_cgoodsRepository.UpdateEntity(modifyentity1)) result.ResultType = OperationResultType.Success;
                   else LogHelper.WriteError("DealCreaditGoodsEdit ==> Del");
                   break;
           }
           return result;
       }


       public CreditGoodsEntity GetEntity(int id)
       {

           return _cgoodsRepository.FindById(id) ?? new CreditGoodsEntity();

       }

       public OperationResult VaildPara(CreditGoodsEntity cgoodEntity)
       {
           if (cgoodEntity.NeedCredits <= 0)
           {
               return new OperationResult(OperationResultType.ParamError, "兑换所需积分必须大于0");
           }
           if (cgoodEntity.Quantity < 0 )
           {
               return new OperationResult(OperationResultType.ParamError, "库存数量必须大于等于0");
           }

           if (cgoodEntity.ExchangeTimes < 0)
           {
               return new OperationResult(OperationResultType.ParamError, "兑换次数必须大于等于0");
           }
           //if (string.IsNullOrEmpty(cgoodEntity.Spec) || cgoodEntity.Spec.Length > 200)
           //{
           //    return new OperationResult(OperationResultType.ParamError, "输入异常，商品规格描述,不能为空，且长度不能超过200字");
           //}
           if (string.IsNullOrEmpty(cgoodEntity.Desc) || cgoodEntity.Spec.Length > 2000)
           {
               return new OperationResult(OperationResultType.ParamError, "输入异常，商品描述,不能为空,长度不能超过2000字。");
           }
           if (cgoodEntity.ExchangeTimes > cgoodEntity.Quantity)
           {
               return new OperationResult(OperationResultType.ParamError,"可兑换次数不应该大于库存数量");
           }
           if (cgoodEntity.StartTime > cgoodEntity.EndTime)
           {
               return new OperationResult(OperationResultType.ParamError,"开始时间不应该大于结束时间");
           }

           return new OperationResult(OperationResultType.Success);




       }

    }
}
