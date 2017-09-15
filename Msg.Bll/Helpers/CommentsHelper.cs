using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Msg.Bll.Adapter;
using Msg.Entities;
using Msg.Providers.Repository;
using Msg.Tools;

namespace Msg.Bll.Helpers
{
    /// <summary>
    /// 评论帮助类
    /// </summary>
    public class CommentsHelper
    {
        private readonly EfRepository<CommentsEntity, int> _commentRepository =
            EfRepository<CommentsEntity, int>.Instance;


        /// <summary>
        /// Gets the new comments count for administrator since last login time.
        /// </summary>
        /// <returns></returns>
        public int GetNewCommentsCountForAdminsSinceLastLoginTime(DateTime lastLoginTime)
        {
            return _commentRepository.LoadEntities(f => f.CreateTime >= lastLoginTime).Count();

        }

        /// <summary>
        /// 获取某用户的评论
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="page">The page.</param>
        /// <param name="size">The size.</param>
        /// <param name="total">The total.</param>
        /// <returns></returns>
        public IQueryable<CommentsEntity> GetUserCommentsEntities(int userId, int page, int size, out int total)
        {
            var query = _commentRepository.LoadEntitiesByPaging(page, size, f => f.User.Id == userId, s => s.Id,
                OrderingOrders.DESC, out total);
            return query;
        }

        /// <summary>
        /// 获取商品评论
        /// </summary>
        /// <param name="goodsId">The goods identifier.</param>
        /// <param name="page">The page.</param>
        /// <param name="size">The size.</param>
        /// <param name="total">The total.</param>
        /// <returns></returns>
        public IQueryable<CommentsEntity> GetGoodsCommentsEntities(int goodsId, int page, int size, out int total)
        {
            var query = _commentRepository.LoadEntitiesByPaging(page, size, f => f.Goods.Id == goodsId, s => s.Id,
              OrderingOrders.ASC, out total);
            return query;
        }


        /// <summary>
        /// 获取积分兑换商品评论
        /// </summary>
        /// <param name="goodsId">The goods identifier.</param>
        /// <param name="page">The page.</param>
        /// <param name="size">The size.</param>
        /// <param name="total">The total.</param>
        /// <returns></returns>
        public IQueryable<CommentsEntity> GetCreditGoodsCommentsEntities(int goodsId, int page, int size, out int total)
        {
            var query = _commentRepository.LoadEntitiesByPaging(page, size, f => f.CreditGoods.Id == goodsId, s => s.Id,
              OrderingOrders.ASC, out total);
            return query;
        }



        /// <summary>
        /// Gets the comments entities.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="total">The total.</param>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        public List<CommentsEntity> GetCommentsEntities(int page, int size, out int total)
        {
            var query = _commentRepository.LoadEntitiesByPaging(page, size, null, s => s.Id, OrderingOrders.DESC,
                out total);
            return query.ToList();

        }

        /// <summary>
        /// Gets the comments entity.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public CommentsEntity GetCommentsEntity(int id)
        {
            if (id <= 0)
            {
                return null;
            }

            return _commentRepository.FindById(id).ProjectedAs<CommentsEntity, int>();

        }


        /// <summary>
        /// Deletes the comment.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public OperationResult DeleteComment(int id)
        {
            if (id <= 0)
            {
                return new OperationResult(OperationResultType.ParamError, "Id");
            }

            var status = _commentRepository.DeleteEntity(id);

            if (status)
            {
                return new OperationResult(OperationResultType.Success);
            }
            return new OperationResult(OperationResultType.NoChanged);
        }

        /// <summary>
        /// Deletes the comment.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public OperationResult DeleteComment(int id, int userId)
        {
            if (userId <= 0)
            {
                return new OperationResult(OperationResultType.ParamError, "用户不存在");
            }
            var comment = _commentRepository.FindById(id);
            if (comment == null)
            {
                return new OperationResult(OperationResultType.QueryNull, "评论不存在");
            }
            if (comment.User.Id != userId)
            {
                return new OperationResult(OperationResultType.PurviewLack);
            }
            var status = _commentRepository.DeleteEntity(comment);

            if (status)
            {
                return new OperationResult(OperationResultType.Success);
            }
            return new OperationResult(OperationResultType.NoChanged);
        }


        /// <summary>
        /// Replies the comment.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="content">The content.</param>
        /// <param name="beReplyedUserId">被评论人Id</param>
        /// <returns></returns>
        public OperationResult ReplyComment(int id, int userId, string content, int? goodsId = null, int beReplyedUserId = -1, string pageUrl = "")
        {
            if (id < 0)
            {
                return new OperationResult(OperationResultType.ParamError, "id");
            }
            if (userId <= 0)
            {
                return new OperationResult(OperationResultType.ParamError, "用户未登录");
            }
            if (string.IsNullOrEmpty(content) || content.Length > 300)
            {
                return new OperationResult(OperationResultType.ParamError, "content");
            }
            var user = UsersHelper.Instance.GetUser(userId);
            if (user == null)
            {
                return new OperationResult(OperationResultType.QueryNull, "用户不存在");
            }
            if (beReplyedUserId == 0 && id == 0 && goodsId.HasValue)//一级评论
            {
                if (goodsId <= 0)
                {
                    return new OperationResult(OperationResultType.ParamError, "商品Id");
                }
                var goods = GoodsHelper.Instance.GetEntity(goodsId);
                if (goods.Id == 0)
                {
                    return new OperationResult(OperationResultType.QueryNull, "商品不存在");
                }

                var comment = _commentRepository.AddEntity(new CommentsEntity() { Content = content, Score = 0, User = user, Goods = goods });

                if (comment.Id > 0)
                {
                    return new OperationResult(OperationResultType.Success, "", comment);
                }
                return new OperationResult(OperationResultType.NoChanged);
            }
            else if (beReplyedUserId > 0 && id > 0)//子级回复
            {
                var parentComment = _commentRepository.FindById(id);
                if (parentComment == null)
                {
                    return new OperationResult(OperationResultType.QueryNull, "父级评论不存在");
                }
                var beReplyedUser = UsersHelper.Instance.GetUser(beReplyedUserId);
                if (beReplyedUser == null)
                {
                    return new OperationResult(OperationResultType.QueryNull, "被回复用户不存在");
                }


                var commentFollows = new CommentsFollowEntity();
                commentFollows.Content = content;
                commentFollows.CreateTime = DateTime.Now;
                commentFollows.User = user;
                commentFollows.ParentComment = parentComment;
                commentFollows.BeReplyedUser = beReplyedUser;
                commentFollows = EfRepository<CommentsFollowEntity, int>.Instance.AddEntity(commentFollows);

                if (commentFollows.Id > 0)
                {
                    Task.Factory.StartNew(() =>
                    {
                        var notify_title = string.Format("{0}回复了您", user.NickName);
                        var notify_content = string.Format("{2}在商品【<a href='/Details/{0}' target='_blank'><span class='cl_0093dd'>{1}</span></a>】" +
                                                           "回复了您的评论，<a href='{4}#comments_reply_{3}' target='_blank'><span class='cl_0093dd'>去看看</span></a>"
                                                           , parentComment.Goods.Id, parentComment.Goods.ShortTitle, user.NickName, commentFollows.Id, pageUrl);
                        NotifiesHelper.Instance.SendNotify(beReplyedUserId, notify_title, notify_content, NotifyTypeEnum.Comment);
                    });

                    return new OperationResult(OperationResultType.Success, "", commentFollows);
                }
                return new OperationResult(OperationResultType.NoChanged);
            }
            return new OperationResult(OperationResultType.IllegalOperation, "未通过验证，暂不能评论");
        }

    }
}
