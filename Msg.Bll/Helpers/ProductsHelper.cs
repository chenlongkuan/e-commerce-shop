using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Practices.Unity;
using Msg.Entities;
using Msg.Providers;
using Msg.Providers.Repository;
using Msg.Providers.UnitOfWork;
using Msg.Tools;
using Msg.Tools.ExpressionExtend;
using Msg.Tools.Logging;

namespace Msg.Bll.Helpers
{
    /// <summary>
    /// 库存商品逻辑帮助类
    /// </summary>
    public class ProductsHelper
    {
        private readonly UnitOfWork _uWork = IoC.Current.Resolve<UnitOfWork>();

        private readonly EfRepository<ProductsEntity, int> _productRepository =
            EfRepository<ProductsEntity, int>.Instance;

        private readonly EfRepository<ProductsApplyEntity, int> _applyRepository =
            EfRepository<ProductsApplyEntity, int>.Instance;

        private readonly EfRepository<SuppliersEntity, int> _supplierRepository =
            EfRepository<SuppliersEntity, int>.Instance;

        private readonly EfRepository<BrandsEntity, int> _brandRepository = EfRepository<BrandsEntity, int>.Instance;


        #region 单例

        private static ProductsHelper _instance;

        public static ProductsHelper Instance
        {
            get { return _instance ?? (_instance = new ProductsHelper()); }
        }

        #endregion


        #region 创业者申请

        /// <summary>
        /// Gets the apply entity.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ProductsApplyEntity GetApplyEntity(int id)
        {
            return id > 0 ? _applyRepository.FindById(id) : new ProductsApplyEntity();
        }

        /// <summary>
        /// 自管理员上次登陆时间之后产生的新入库申请数量
        /// </summary>
        /// <param name="lastLoginTime">管理员上次登陆时间</param>
        /// <returns></returns>
        public int GetNewApplyCountForAdminsSinceLastLoginTime(DateTime lastLoginTime)
        {
            return _applyRepository.LoadEntities(f => f.CreateTime >= lastLoginTime).Count();
        }

        /// <summary>
        /// 获取某创业者的入库商品数量
        /// </summary>
        /// <param name="supplierId"></param>
        /// <returns></returns>
        public int GetSuppliersProductCount(int supplierId)
        {
            return _productRepository.LoadEntities(f => f.Suppliers.Id == supplierId).Count();
        }

        /// <summary>
        /// 获取某用户的创业集市产品申请记录
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="page">The page.</param>
        /// <param name="size">The size.</param>
        /// <param name="total">The total.</param>
        /// <returns></returns>
        public IQueryable<ProductsApplyEntity> GetApplyEntities(int userId, int page, int size, out int total)
        {
            return _applyRepository.LoadEntitiesByPaging(page, size, f => f.Supplier.User.Id == userId, s => s.ApplyStatus,
                OrderingOrders.DESC, out total);
        }

        /// <summary>
        /// 添加创业集市产品申请
        /// </summary>
        /// <param name="apply">The apply.</param>
        /// <param name="userId">创业者用户Id</param>
        /// <returns></returns>
        public OperationResult AddProductApply(ProductsApplyEntity apply, int userId, int categoryId)
        {
            SuppliersEntity supplier;
            CategoriesEntity categories;
            OperationResult operationResult;
            if (!ValidProductApplyParams(apply, userId, out supplier, out operationResult)) return operationResult;
            if (categoryId <= 0)
            {
                return new OperationResult(OperationResultType.ParamError, "产品类目", apply);
            }
            categories = CategoryHelper.Instance.GetEntityForMarketById(categoryId);
            if (categories == null)
            {
                return new OperationResult(OperationResultType.QueryNull, "产品类目不存在", apply);
            }

            apply.Category = categories;
            apply.Supplier = supplier;
            apply.ApplyStatus = (int)ApplyStatusEnum.Verifying;
            apply.IsUseable = true;
            apply = _applyRepository.AddEntity(apply);

            return new OperationResult(apply.Id > 0 ? OperationResultType.Success : OperationResultType.NoChanged, "", apply);
        }

        /// <summary>
        /// 编辑产品申请
        /// </summary>
        /// <param name="apply">The apply.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public OperationResult ModifyProductApply(ProductsApplyEntity apply, int userId, int categoryId)
        {
            if (apply.Id <= 0)
            {
                return new OperationResult(OperationResultType.ParamError, "Id", apply);
            }

            SuppliersEntity supplier;
            CategoriesEntity categories;
            OperationResult operationResult;
            if (!ValidProductApplyParams(apply, userId, out supplier, out operationResult)) return operationResult;
            if (categoryId <= 0)
            {
                return new OperationResult(OperationResultType.ParamError, "产品类目", apply);
            }
            categories = CategoryHelper.Instance.GetEntityForMarketById(categoryId);
            if (categories == null)
            {
                return new OperationResult(OperationResultType.QueryNull, "产品类目不存在", apply);
            }


            var entity = GetApplyEntity(apply.Id);
            entity.ApplyStatus = (int)ApplyStatusEnum.Verifying;
            entity.Name = apply.Name;
            entity.Price = apply.Price;
            entity.Quantity = apply.Quantity;
            entity.Spec = apply.Spec;
            entity.Desc = apply.Desc;
            //entity.BrandName = apply.BrandName;
            entity.Logo = apply.Logo;
            entity.Category = categories;
            var status = _applyRepository.UpdateEntity(entity);

            return new OperationResult(status ? OperationResultType.Success : OperationResultType.NoChanged, "", entity);

        }

        /// <summary>
        /// Valids the product apply parameters.
        /// </summary>
        /// <param name="apply">The apply.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="supplier">The supplier.</param>
        /// <param name="operationResult">The operation result.</param>
        /// <returns></returns>
        private bool ValidProductApplyParams(ProductsApplyEntity apply, int userId, out SuppliersEntity supplier,
            out OperationResult operationResult)
        {
            var status = false;
            operationResult = null;
            if (userId <= 0)
            {
                {
                    operationResult = new OperationResult(OperationResultType.ParamError, "用户Id", apply);

                }
            }
            //if (string.IsNullOrEmpty(apply.Name))
            //{
            //    {
            //        operationResult = new OperationResult(OperationResultType.ParamError, "产品名称");

            //    }
            //}

            if (string.IsNullOrEmpty(apply.BrandName))
            {
                {
                    operationResult = new OperationResult(OperationResultType.ParamError, "品牌名称", apply);

                }
            }
            if (apply.Price <= 0)
            {
                {
                    operationResult = new OperationResult(OperationResultType.ParamError, "产品价格", apply);

                }
            }
            if (apply.Quantity <= 0)
            {
                {
                    operationResult = new OperationResult(OperationResultType.ParamError, "入库数量", apply);

                }
            }
            if (string.IsNullOrEmpty(apply.Logo))
            {
                {
                    operationResult = new OperationResult(OperationResultType.ParamError, "产品logo", apply);

                }
            }

            supplier = _supplierRepository.LoadEntities(f => f.User.Id == userId, false).SingleOrDefault();
            if (supplier == null)
            {
                {
                    operationResult = new OperationResult(OperationResultType.PurviewLack, "用户不存在", apply);

                }
            }
            if (supplier != null && (!supplier.User.IsActive || !supplier.User.IsUseable))
            {
                {
                    operationResult = new OperationResult(OperationResultType.PurviewLack, "用户未激活或已被锁定", apply);

                }
            }
            if (supplier != null && !supplier.IsVerified)
            {
                {
                    operationResult = new OperationResult(OperationResultType.PurviewLack, "创业者还未通过审核", apply);

                }
            }

            return status;
        }

        /// <summary>
        ///通过创业集市产品入库申请
        /// </summary>
        /// <param name="applyId">The apply identifier.</param>
        /// <param name="remark">The remark.</param>
        /// <returns></returns>
        public OperationResult PassProductApply(int applyId, string remark)
        {
            return UpdateProductApplyStatus(applyId, remark, ApplyStatusEnum.ApplyFailed);
        }

        /// <summary>
        /// 拒绝创业集市产品入库申请
        /// </summary>
        /// <param name="applyId">The apply identifier.</param>
        /// <param name="remark">The remark.</param>
        /// <returns></returns>
        public OperationResult RefuseProductApply(int applyId, string remark)
        {
            if (string.IsNullOrEmpty(remark))
            {
                return new OperationResult(OperationResultType.ParamError, "拒绝理由");
            }
            return UpdateProductApplyStatus(applyId, remark, ApplyStatusEnum.ApplyFailed);
        }


        /// <summary>
        /// 删除创业产品申请记录
        /// </summary>
        /// <param name="applyId">The apply identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public OperationResult DeleteProductApply(int applyId, int userId)
        {
            if (applyId <= 0)
            {
                return new OperationResult(OperationResultType.ParamError, "applyId");
            }
            var apply = _applyRepository.FindById(applyId);
            if (apply != null && apply.ApplyStatus == (byte)ApplyStatusEnum.ApplyFailed && apply.Supplier.User.Id == userId)
            {
                var status = _applyRepository.DeleteEntity(apply);
                return new OperationResult(status ? OperationResultType.Success : OperationResultType.NoChanged);
            }

            return new OperationResult(OperationResultType.PurviewLack);
        }

        /// <summary>
        /// Updates the product apply status.
        /// </summary>
        /// <param name="applyId">The apply identifier.</param>
        /// <param name="remark">The remark.</param>
        /// <param name="applyStatus">The apply status.</param>
        /// <returns></returns>
        private OperationResult UpdateProductApplyStatus(int applyId, string remark, ApplyStatusEnum applyStatus)
        {
            if (applyId <= 0)
            {
                return new OperationResult(OperationResultType.ParamError, "applyId");
            }
            var apply = _applyRepository.FindById(applyId);
            if (apply != null)
            {
                apply.ApplyStatus = (byte)applyStatus;
                apply.OperateRemark = remark;
            }
            else
            {
                return new OperationResult(OperationResultType.QueryNull, "查无此记录");
            }
            var status = _applyRepository.UpdateEntity(apply);

            return new OperationResult(status ? OperationResultType.Success : OperationResultType.NoChanged);
        }

        #endregion


        #region 创业产品申请


        /// <summary>
        /// Gets the products apply entities.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="brandName">Name of the brand.</param>
        /// <param name="statusEnum">The status enum.</param>
        /// <param name="page">The page.</param>
        /// <param name="size">The size.</param>
        /// <param name="total">The total.</param>
        /// <returns></returns>
        public IQueryable<ProductsApplyEntity> GetProductsApplyEntities(string name, string supplierName, string brandName, int? cateId, ApplyStatusEnum? statusEnum, int page, int size, out int total)
        {
            Expression<Func<ProductsApplyEntity, bool>> filter = null;
            Expression<Func<ProductsApplyEntity, int>> sortCondition = s => s.Id;

            if (!string.IsNullOrEmpty(name))
            {
                filter = f => f.Name == name;
            }
            if (!string.IsNullOrEmpty(supplierName))
            {
                filter = filter == null
                    ? f => f.Supplier.User.NickName == supplierName
                    : filter.And(f => f.Supplier.User.NickName == supplierName);
            }
            if (!string.IsNullOrEmpty(brandName))
            {
                filter = filter == null ? f => f.BrandName == brandName : filter.And(f => f.BrandName == brandName);
            }
            if (cateId.HasValue)
            {
                filter = filter == null ? f => f.Category.Id == cateId : filter.And(f => f.Category.Id == cateId);
            }
            if (statusEnum.HasValue)
            {
                filter = filter == null
                    ? f => f.ApplyStatus == (byte)statusEnum
                    : filter.And(f => f.ApplyStatus == (byte)statusEnum);
            }

            return _applyRepository.LoadEntitiesByPaging(page, size, filter, sortCondition, OrderingOrders.DESC,
                out total);


        }


        /// <summary>
        /// Products the authority.
        /// </summary>
        /// <param name="applyId">The apply identifier.</param>
        /// <param name="statusEnum">The status enum.</param>
        /// <returns></returns>
        public OperationResult ProductAuthority(int applyId, ApplyStatusEnum statusEnum, string remark)
        {
            try
            {
                var apply = _applyRepository.FindById(applyId);
                if (apply != null)
                {
                    apply.OperateRemark = remark;
                    apply.ApplyStatus = (byte)statusEnum;
                    if (statusEnum.Equals(ApplyStatusEnum.ApplySuccess))
                    {
                        var brand = _brandRepository.LoadEntities(f => f.Name == apply.Name).FirstOrDefault();
                        if (brand == null)
                        {
                            brand = new BrandsEntity();
                            brand.Name = apply.BrandName;

                            brand = _brandRepository.AddEntity(brand, false);
                        }
                        var product = new ProductsEntity();
                        product.Name = apply.Name;
                        product.Logo = apply.Logo;
                        product.IsVirtual = false;
                        product.Price = apply.Price;
                        product.Quantity = apply.Quantity;
                        product.Spec = apply.Spec;
                        product.Desc = apply.Desc;
                        product.Suppliers = apply.Supplier;
                        product.ApplyEntity = apply;
                        product.Brand = brand;
                        product.Category = apply.Category;

                        _productRepository.AddEntity(product, false);
                    }

                    var notifytCotent = string.Format("亲爱的创业者，你申请售卖的产品【{0}】{1} 管理员审核,操作备注：{2}"
                        , apply.Name, statusEnum.Equals(ApplyStatusEnum.ApplySuccess) ? "通过了" : "未通过", remark);
                    NotifiesHelper.Instance.SendNotify(apply.Supplier.User.Id, "产品申请审核结果", notifytCotent, NotifyTypeEnum.Admin, false);

                    _applyRepository.UpdateEntity(apply, false);

                    var effectRows = _uWork.SaveChanges();

                    return new OperationResult(effectRows > 0 ? OperationResultType.Success : OperationResultType.NoChanged);
                }

                return new OperationResult(OperationResultType.QueryNull, "申请记录");
            }
            catch (Exception ex)
            {
                LogHelper.WriteException("ProductHelper->ProductAuthority:", ex);
                return new OperationResult(OperationResultType.Error, ex.Message);
            }

        }

        #endregion

        /// <summary>
        /// Gets the entity.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ProductsEntity GetEntity(int? id)
        {
            return id.HasValue ? _productRepository.FindById(id.Value) : new ProductsEntity();
        }


        /// <summary>
        /// Gets the products entities.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="brandId">The brand identifier.</param>
        /// <param name="cateId">The cate identifier.</param>
        /// <param name="sortColumn">排序字段</param>
        /// <param name="page">The page.</param>
        /// <param name="size">The size.</param>
        /// <param name="total">The total.</param>
        /// <returns></returns>
        public IQueryable<ProductsEntity> GetProductsEntities(string name, int? brandId, int? cateId, int? supplierId, string sortColumn, int page, int size,
            out int total)
        {
            Expression<Func<ProductsEntity, bool>> filter = null;
            Expression<Func<ProductsEntity, int>> sortCondition;

            if (!string.IsNullOrEmpty(name))
            {
                filter = f => f.Name.Contains(name);
            }
            if (brandId.HasValue)
            {
                filter = filter == null ? f => f.Brand.Id == brandId : filter.And(f => f.Brand.Id == brandId);
            }

            if (cateId.HasValue)
            {
                filter = filter == null ? f => f.Category.Id == cateId : filter.And(f => f.Category.Id == cateId);
            }
            if (supplierId.HasValue)
            {
                filter = filter == null ? f => f.Suppliers.Id == supplierId : filter.And(f => f.Suppliers.Id == supplierId);
            }
            switch (sortColumn)
            {
                case "Quantity":
                    sortCondition = s => s.Quantity;
                    break;
                default:
                    sortCondition = s => s.Id;
                    break;
            }
            var query = _productRepository.LoadEntitiesByPaging(page, size, filter, sortCondition, OrderingOrders.DESC,
                        out total);

            return query;
        }


        /// <summary>
        /// Save the product.
        /// </summary>
        /// <param name="product">The product.</param>
        /// <param name="cateId">类目Id</param>
        /// <param name="brandId">品牌Id</param>
        /// <returns></returns>
        public OperationResult SaveProduct(ProductsEntity product, int cateId, int brandId)
        {
            PublicHelper.CheckArgument(product, "product");

            try
            {
                var saveProduct = ValidParams(product, cateId, brandId);
                if (saveProduct != null) return saveProduct;

                product.Category = CategoryHelper.Instance.GetEntity(cateId);
                product.Brand = BrandsHelper.Instance.GetEntity(brandId);

                if (product.Id == 0)//新增
                {
                    product = _productRepository.AddEntity(product);
                    if (product.Id > 0)
                    {
                        return new OperationResult(OperationResultType.Success, "", product);
                    }
                    return new OperationResult(OperationResultType.NoChanged, "操作失败", product);

                }
                else if (product.Id > 0)//编辑
                {
                    var entity = _productRepository.FindById(product.Id);
                    if (entity == null)
                    {
                        return new OperationResult(OperationResultType.QueryNull, "被编辑产品", product);
                    }

                    entity.Name = product.Name;
                    entity.Price = product.Price;
                    entity.Logo = product.Logo;
                    entity.IsVirtual = product.IsVirtual;
                    entity.Quantity = product.Quantity;
                    entity.Category = product.Category;
                    entity.Brand = product.Brand;
                    entity.Venue = product.Venue;
                    entity.UsingDate = product.UsingDate;
                    entity.Destination = product.Destination;
                    entity.Spec = product.Spec;
                    entity.Desc = product.Desc;


                    if (entity.Category == null)
                    {
                        return new OperationResult(OperationResultType.QueryNull, "Category", "", product);
                    }

                    var status = _productRepository.UpdateEntity(entity);
                    if (status)
                    {
                        return new OperationResult(OperationResultType.Success, "", product);
                    }
                    return new OperationResult(OperationResultType.NoChanged, "操作失败", product);
                }

                return new OperationResult(OperationResultType.IllegalOperation);
            }
            catch (Exception ex)
            {
                LogHelper.WriteException("SaveProduct", ex);
                return new OperationResult(OperationResultType.Error, "", product);
            }


        }

        /// <summary>
        /// 切换产品的可用状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public OperationResult ToggleUseable(int id)
        {
            if (id <= 0)
            {
                return new OperationResult(OperationResultType.ParamError, "Id");
            }

            var product = _productRepository.FindById(id);
            if (product == null)
            {
                return new OperationResult(OperationResultType.QueryNull, "Product");
            }

            product.IsUseable = !product.IsUseable;

            var status = _productRepository.UpdateEntity(product);

            return new OperationResult(status ? OperationResultType.Success : OperationResultType.NoChanged);

        }


        /// <summary>
        /// Valids the parameters.
        /// </summary>
        /// <param name="product">The product.</param>
        /// <param name="cateId">The cate identifier.</param>
        /// <param name="brandId">The brand identifier.</param>
        /// <returns></returns>
        private OperationResult ValidParams(ProductsEntity product, int cateId, int brandId)
        {
            OperationResult saveProduct = null;
            if (string.IsNullOrEmpty(product.Name) || product.Name.Length > 100)
            {
                saveProduct = new OperationResult(OperationResultType.ParamError, "Name",product);

            }
            if (product.Price <= 0)
            {
                saveProduct = new OperationResult(OperationResultType.ParamError, "Price", product);

            }
            if (cateId <= 0)
            {
                saveProduct = new OperationResult(OperationResultType.ParamError, "CateId", product);

            }
            //if (brandId <= 0)
            //{
            //    saveProduct = new OperationResult(OperationResultType.ParamError, "BrandId");

            //}
            if (product.Quantity <= 0)
            {
                saveProduct = new OperationResult(OperationResultType.ParamError, "Quantity", product);

            }
            if (string.IsNullOrEmpty(product.Logo))
            {
                saveProduct = new OperationResult(OperationResultType.ParamError, "Logo", product);

            }
            if (product.Spec != null && product.Spec.Length > 2000)
            {
                saveProduct = new OperationResult(OperationResultType.ParamError, "规格字数超出长度", product);
            }
            if (product.Desc != null && product.Desc.Length > 2000)
            {
                saveProduct = new OperationResult(OperationResultType.ParamError, "描述字数超出长度", product);
            }

            return saveProduct;
        }
    }
}
