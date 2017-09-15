using System.Linq;
using Microsoft.Practices.Unity;
using Msg.Entities;
using Msg.Providers;
using Msg.Providers.Repository;
using Msg.Providers.UnitOfWork;
using Msg.Tools;
using Msg.Tools.Extensions;

namespace Msg.Bll.Helpers
{
    /// <summary>
    /// 通知消息（站内信）业务帮助类
    /// </summary>
    public class NotifiesHelper
    {
        private readonly UnitOfWork _unitOfWork = IoC.Current.Resolve<UnitOfWork>();
        private readonly EfRepository<NotifiesEntity, int> _notifyRepository = EfRepository<NotifiesEntity, int>.Instance;

        #region 单例

        private static NotifiesHelper _instance;

        public static NotifiesHelper Instance
        {
            get { return _instance ?? (_instance = new NotifiesHelper()); }
        }

        #endregion

        /// <summary>
        /// Gets the notifies entities.
        /// </summary>
        /// <param name="receiverId">The receiver identifier.</param>
        /// <param name="page">The page.</param>
        /// <param name="size">The size.</param>
        /// <param name="total">The total.</param>
        /// <returns></returns>
        public IQueryable<NotifiesEntity> GetNotifiesEntities(int receiverId, int page, int size, out int total)
        {
            var query = _notifyRepository.LoadEntitiesByPaging(page, size, f => f.User.Id == receiverId, s => s.Id,
                OrderingOrders.DESC, out total);
            return query;
        }

        /// <summary>
        /// 发送通知
        /// </summary>
        /// <param name="toUserId">接受通知人用户Id</param>
        /// <param name="title">通知标题</param>
        /// <param name="content">通知内容</param>
        /// <param name="typeEnum">通知类型</param>
        /// <returns></returns>
        public OperationResult SendNotify(int toUserId, string title, string content, NotifyTypeEnum typeEnum, bool immediatelyCommit = true)
        {
            if (toUserId <= 0)
            {
                return new OperationResult(OperationResultType.ParamError, "toUserId");
            }
            if (string.IsNullOrEmpty(title) || title.Length > 50)
            {
                title = typeEnum.ToDescription();
            }

            if (string.IsNullOrEmpty(content) || content.Length > 300)
            {
                return new OperationResult(OperationResultType.ParamError, "content");
            }
            var user = EfRepository<UsersEntity, int>.Instance.FindById(toUserId);
            if (user == null)
            {
                return new OperationResult(OperationResultType.QueryNull, "user");
            }

            var notify = new NotifiesEntity() { Content = content, Title = title, Type = (byte)typeEnum, User = user };

            notify = _notifyRepository.AddEntity(notify,immediatelyCommit);

            if (notify.Id > 0)
            {
                return new OperationResult(OperationResultType.Success, "", notify);
            }
            return new OperationResult(OperationResultType.NoChanged);

        }


        /// <summary>
        /// 批量发送站内信
        /// </summary>
        /// <param name="toUserIds">To user ids.</param>
        /// <param name="title">The title.</param>
        /// <param name="content">The content.</param>
        /// <param name="typeEnum">The type enum.</param>
        /// <returns></returns>
        public OperationResult SendNotifyBatch(string toUserIds, string title, string content, NotifyTypeEnum typeEnum)
        {
            if (string.IsNullOrEmpty(toUserIds))
            {
                return new OperationResult(OperationResultType.ParamError, "请选择用户");
            }

            if (toUserIds.Contains(","))
            {
                var toUIdList = toUserIds.Split(',').ToList();
                foreach (var userId in toUIdList)
                {
                    SendNotify(int.Parse(userId), title, content, typeEnum, false);
                }
            }
            else
            {
                SendNotify(int.Parse(toUserIds), title, content, typeEnum, false);
            }
            var effectRows = _unitOfWork.SaveChanges();
            return new OperationResult(effectRows > 0 ? OperationResultType.Success : OperationResultType.NoChanged);
        }


        /// <summary>
        /// Deletes the notify.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="receiverId">The receiver identifier.</param>
        /// <returns></returns>
        public OperationResult DeleteNotify(int id, int receiverId)
        {
            if (receiverId <= 0)
            {
                return new OperationResult(OperationResultType.ParamError, "用户不存在");
            }
            var notify = _notifyRepository.FindById(id);
            if (notify == null)
            {
                return new OperationResult(OperationResultType.QueryNull, "站内信不存在");
            }
            if (notify.User.Id != receiverId)
            {
                return new OperationResult(OperationResultType.PurviewLack);
            }
            var status = _notifyRepository.DeleteEntity(notify);

            if (status)
            {
                return new OperationResult(OperationResultType.Success);
            }
            return new OperationResult(OperationResultType.NoChanged);
        }

        /// <summary>
        /// 改变站内信状态为已读
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public OperationResult ToBeRead(int userId, int id)
        {
            if (id <= 0)
            {
                return new OperationResult(OperationResultType.ParamError, "Id");
            }
            var notify = _notifyRepository.FindById(id);
            if (notify == null)
            {
                return new OperationResult(OperationResultType.QueryNull, "站内信不存在");
            }
            if (notify.User.Id != userId)
            {
                return new OperationResult(OperationResultType.PurviewLack);
            }
            if (notify.State == 1)
            {
                return new OperationResult(OperationResultType.NoChanged);
            }
            notify.State = 1;
            var status = _notifyRepository.UpdateEntity(notify);
            if (status)
            {
                return new OperationResult(OperationResultType.Success);
            }
            return new OperationResult(OperationResultType.NoChanged);
        }

    }
}
