using System;
using System.Linq;
using Msg.Bll.Helpers;
using Msg.Entities;
using Msg.Providers.Repository;
using Msg.Tools;

namespace Msg.Bll
{
    public class SuppliersHelper
    {
        private readonly EfRepository<SuppliersEntity, int> _supplierRepository =
            EfRepository<SuppliersEntity, int>.Instance;

        /// <summary>
        /// 是否通过了创业者申请
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public bool IsSupplierAuthorized(int userId)
        {
            var supplier = _supplierRepository.LoadEntities(f => f.User.Id == userId).SingleOrDefault();
            return supplier != null && supplier.IsVerified;
        }


        /// <summary>
        /// 认证创业者是否具备新申请/再申请条件
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public OperationResult AuthorizeSupplier(int userId)
        {
            var supplier = _supplierRepository.LoadEntities(f => f.User.Id == userId).SingleOrDefault();
            if (supplier == null)
            {
                return new OperationResult(OperationResultType.Success, "可提交申请");
            }
            else
            {
                //之前未审核通过的，超过7天可重复申请，并删除老的申请数据
                if (!supplier.IsVerified)
                {
                    var dayDiff = Utils.Utils.DateDayDiff(supplier.CreateTime, DateTime.Now);
                    if (dayDiff >= 7)
                    {
                        var status = _supplierRepository.DeleteEntity(supplier);
                        if (status)
                        {
                            return new OperationResult(OperationResultType.Success, "可提交申请");
                        }
                        return new OperationResult(OperationResultType.Error, "不可提交申请，请稍后再试");
                    }
                    else
                    {
                        return new OperationResult(OperationResultType.IllegalOperation, string.Format("{0}天之后可再次提交申请", dayDiff));
                    }

                }
                else
                {
                    return new OperationResult(OperationResultType.NoChanged, "已通过了申请", supplier);
                }
            }

        }

        /// <summary>
        /// 添加成为创业者
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="linkMan">The link man.</param>
        /// <param name="linkTel">The link tel.</param>
        /// <param name="reason">The reason.</param>
        /// <returns></returns>
        public OperationResult AddToBeSupplier(int userId, string linkMan, string linkTel, string reason)
        {
            if (userId <= 0)
            {
                return new OperationResult(OperationResultType.ParamError, "用户参数");
            }
            if (string.IsNullOrEmpty(linkMan) || linkMan.Length > 10)
            {
                return new OperationResult(OperationResultType.ParamError, "联系人");
            }
            if (string.IsNullOrEmpty(linkTel) || linkTel.Length > 11)
            {
                return new OperationResult(OperationResultType.ParamError, "联系电话");
            }
            if (string.IsNullOrEmpty(reason) || linkTel.Length > 500)
            {
                return new OperationResult(OperationResultType.ParamError, "申请原因");
            }
            var user = UsersHelper.Instance.GetUser(userId);
            if (user == null || !user.IsActive || !user.IsUseable)
            {
                return new OperationResult(OperationResultType.IllegalOperation, "用户不可用");
            }

            var supplier = new SuppliersEntity()
            {
                Reason = reason,
                LinkMan = linkMan,
                LinkTel = linkTel,
                User = user
            };

            supplier = _supplierRepository.AddEntity(supplier);
            if (supplier.Id > 0)
            {
                return new OperationResult(OperationResultType.Success, "", supplier);
            }
            return new OperationResult(OperationResultType.NoChanged);

        }

    }
}
