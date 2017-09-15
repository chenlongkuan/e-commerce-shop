﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Msg.FileUpload.WcfUpload {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="WcfUpload.IUpload")]
    public interface IUpload {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUpload/UploadFile", ReplyAction="http://tempuri.org/IUpload/UploadFileResponse")]
        WCF.Lib.File.Entity.Message UploadFile(WCF.Lib.File.Entity.AttachmentsEntity attachmen);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUpload/UploadFile", ReplyAction="http://tempuri.org/IUpload/UploadFileResponse")]
        System.Threading.Tasks.Task<WCF.Lib.File.Entity.Message> UploadFileAsync(WCF.Lib.File.Entity.AttachmentsEntity attachmen);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUpload/Deletefile", ReplyAction="http://tempuri.org/IUpload/DeletefileResponse")]
        int Deletefile(string filename, string salt);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUpload/Deletefile", ReplyAction="http://tempuri.org/IUpload/DeletefileResponse")]
        System.Threading.Tasks.Task<int> DeletefileAsync(string filename, string salt);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUpload/MovePhoto", ReplyAction="http://tempuri.org/IUpload/MovePhotoResponse")]
        bool MovePhoto(int uid, int albumid, string filename, int newAlbumid);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUpload/MovePhoto", ReplyAction="http://tempuri.org/IUpload/MovePhotoResponse")]
        System.Threading.Tasks.Task<bool> MovePhotoAsync(int uid, int albumid, string filename, int newAlbumid);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUpload/SetAvatar", ReplyAction="http://tempuri.org/IUpload/SetAvatarResponse")]
        string SetAvatar(int uid, int avatartype, byte[] data);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUpload/SetAvatar", ReplyAction="http://tempuri.org/IUpload/SetAvatarResponse")]
        System.Threading.Tasks.Task<string> SetAvatarAsync(int uid, int avatartype, byte[] data);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUpload/WeiboAvatar", ReplyAction="http://tempuri.org/IUpload/WeiboAvatarResponse")]
        Msg.FileUpload.WcfUpload.WeiboAvatarResponse WeiboAvatar(Msg.FileUpload.WcfUpload.WeiboAvatarRequest request);
        
        // CODEGEN: Generating message contract since the operation has multiple return values.
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUpload/WeiboAvatar", ReplyAction="http://tempuri.org/IUpload/WeiboAvatarResponse")]
        System.Threading.Tasks.Task<Msg.FileUpload.WcfUpload.WeiboAvatarResponse> WeiboAvatarAsync(Msg.FileUpload.WcfUpload.WeiboAvatarRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUpload/SavePhoto", ReplyAction="http://tempuri.org/IUpload/SavePhotoResponse")]
        WCF.Lib.File.Entity.Message SavePhoto(int uid, int albumid, byte[] data, string filename);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUpload/SavePhoto", ReplyAction="http://tempuri.org/IUpload/SavePhotoResponse")]
        System.Threading.Tasks.Task<WCF.Lib.File.Entity.Message> SavePhotoAsync(int uid, int albumid, byte[] data, string filename);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUpload/deletePhoto", ReplyAction="http://tempuri.org/IUpload/deletePhotoResponse")]
        bool deletePhoto(int uid, int albumid, string filename);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUpload/deletePhoto", ReplyAction="http://tempuri.org/IUpload/deletePhotoResponse")]
        System.Threading.Tasks.Task<bool> deletePhotoAsync(int uid, int albumid, string filename);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUpload/deleteAlbum", ReplyAction="http://tempuri.org/IUpload/deleteAlbumResponse")]
        bool deleteAlbum(int uid, int albumid);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUpload/deleteAlbum", ReplyAction="http://tempuri.org/IUpload/deleteAlbumResponse")]
        System.Threading.Tasks.Task<bool> deleteAlbumAsync(int uid, int albumid);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUpload/UploadMakeThumbnailImage", ReplyAction="http://tempuri.org/IUpload/UploadMakeThumbnailImageResponse")]
        WCF.Lib.File.Entity.Message UploadMakeThumbnailImage(WCF.Lib.File.Entity.AttachmentsEntity attachmen, int maxHeight, int maxWidth);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUpload/UploadMakeThumbnailImage", ReplyAction="http://tempuri.org/IUpload/UploadMakeThumbnailImageResponse")]
        System.Threading.Tasks.Task<WCF.Lib.File.Entity.Message> UploadMakeThumbnailImageAsync(WCF.Lib.File.Entity.AttachmentsEntity attachmen, int maxHeight, int maxWidth);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUpload/MakeSmailImageByMaxWidthLogo", ReplyAction="http://tempuri.org/IUpload/MakeSmailImageByMaxWidthLogoResponse")]
        WCF.Lib.File.Entity.Message MakeSmailImageByMaxWidthLogo(WCF.Lib.File.Entity.AttachmentsEntity attachmen, int maxWidth, string logo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUpload/MakeSmailImageByMaxWidthLogo", ReplyAction="http://tempuri.org/IUpload/MakeSmailImageByMaxWidthLogoResponse")]
        System.Threading.Tasks.Task<WCF.Lib.File.Entity.Message> MakeSmailImageByMaxWidthLogoAsync(WCF.Lib.File.Entity.AttachmentsEntity attachmen, int maxWidth, string logo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUpload/MakeSmailImageByMaxWidth", ReplyAction="http://tempuri.org/IUpload/MakeSmailImageByMaxWidthResponse")]
        WCF.Lib.File.Entity.Message MakeSmailImageByMaxWidth(WCF.Lib.File.Entity.AttachmentsEntity attachmen, int maxWidth);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUpload/MakeSmailImageByMaxWidth", ReplyAction="http://tempuri.org/IUpload/MakeSmailImageByMaxWidthResponse")]
        System.Threading.Tasks.Task<WCF.Lib.File.Entity.Message> MakeSmailImageByMaxWidthAsync(WCF.Lib.File.Entity.AttachmentsEntity attachmen, int maxWidth);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="WeiboAvatar", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class WeiboAvatarRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public int uid;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=1)]
        public string Avatarurl;
        
        public WeiboAvatarRequest() {
        }
        
        public WeiboAvatarRequest(int uid, string Avatarurl) {
            this.uid = uid;
            this.Avatarurl = Avatarurl;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="WeiboAvatarResponse", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class WeiboAvatarResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public bool WeiboAvatarResult;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=1)]
        public string msg;
        
        public WeiboAvatarResponse() {
        }
        
        public WeiboAvatarResponse(bool WeiboAvatarResult, string msg) {
            this.WeiboAvatarResult = WeiboAvatarResult;
            this.msg = msg;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IUploadChannel : Msg.FileUpload.WcfUpload.IUpload, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class UploadClient : System.ServiceModel.ClientBase<Msg.FileUpload.WcfUpload.IUpload>, Msg.FileUpload.WcfUpload.IUpload {
        
        public UploadClient() {
        }
        
        public UploadClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public UploadClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public UploadClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public UploadClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public WCF.Lib.File.Entity.Message UploadFile(WCF.Lib.File.Entity.AttachmentsEntity attachmen) {
            return base.Channel.UploadFile(attachmen);
        }
        
        public System.Threading.Tasks.Task<WCF.Lib.File.Entity.Message> UploadFileAsync(WCF.Lib.File.Entity.AttachmentsEntity attachmen) {
            return base.Channel.UploadFileAsync(attachmen);
        }
        
        public int Deletefile(string filename, string salt) {
            return base.Channel.Deletefile(filename, salt);
        }
        
        public System.Threading.Tasks.Task<int> DeletefileAsync(string filename, string salt) {
            return base.Channel.DeletefileAsync(filename, salt);
        }
        
        public bool MovePhoto(int uid, int albumid, string filename, int newAlbumid) {
            return base.Channel.MovePhoto(uid, albumid, filename, newAlbumid);
        }
        
        public System.Threading.Tasks.Task<bool> MovePhotoAsync(int uid, int albumid, string filename, int newAlbumid) {
            return base.Channel.MovePhotoAsync(uid, albumid, filename, newAlbumid);
        }
        
        public string SetAvatar(int uid, int avatartype, byte[] data) {
            return base.Channel.SetAvatar(uid, avatartype, data);
        }
        
        public System.Threading.Tasks.Task<string> SetAvatarAsync(int uid, int avatartype, byte[] data) {
            return base.Channel.SetAvatarAsync(uid, avatartype, data);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Msg.FileUpload.WcfUpload.WeiboAvatarResponse Msg.FileUpload.WcfUpload.IUpload.WeiboAvatar(Msg.FileUpload.WcfUpload.WeiboAvatarRequest request) {
            return base.Channel.WeiboAvatar(request);
        }
        
        public bool WeiboAvatar(int uid, string Avatarurl, out string msg) {
            Msg.FileUpload.WcfUpload.WeiboAvatarRequest inValue = new Msg.FileUpload.WcfUpload.WeiboAvatarRequest();
            inValue.uid = uid;
            inValue.Avatarurl = Avatarurl;
            Msg.FileUpload.WcfUpload.WeiboAvatarResponse retVal = ((Msg.FileUpload.WcfUpload.IUpload)(this)).WeiboAvatar(inValue);
            msg = retVal.msg;
            return retVal.WeiboAvatarResult;
        }
        
        public System.Threading.Tasks.Task<Msg.FileUpload.WcfUpload.WeiboAvatarResponse> WeiboAvatarAsync(Msg.FileUpload.WcfUpload.WeiboAvatarRequest request) {
            return base.Channel.WeiboAvatarAsync(request);
        }
        
        public WCF.Lib.File.Entity.Message SavePhoto(int uid, int albumid, byte[] data, string filename) {
            return base.Channel.SavePhoto(uid, albumid, data, filename);
        }
        
        public System.Threading.Tasks.Task<WCF.Lib.File.Entity.Message> SavePhotoAsync(int uid, int albumid, byte[] data, string filename) {
            return base.Channel.SavePhotoAsync(uid, albumid, data, filename);
        }
        
        public bool deletePhoto(int uid, int albumid, string filename) {
            return base.Channel.deletePhoto(uid, albumid, filename);
        }
        
        public System.Threading.Tasks.Task<bool> deletePhotoAsync(int uid, int albumid, string filename) {
            return base.Channel.deletePhotoAsync(uid, albumid, filename);
        }
        
        public bool deleteAlbum(int uid, int albumid) {
            return base.Channel.deleteAlbum(uid, albumid);
        }
        
        public System.Threading.Tasks.Task<bool> deleteAlbumAsync(int uid, int albumid) {
            return base.Channel.deleteAlbumAsync(uid, albumid);
        }
        
        public WCF.Lib.File.Entity.Message UploadMakeThumbnailImage(WCF.Lib.File.Entity.AttachmentsEntity attachmen, int maxHeight, int maxWidth) {
            return base.Channel.UploadMakeThumbnailImage(attachmen, maxHeight, maxWidth);
        }
        
        public System.Threading.Tasks.Task<WCF.Lib.File.Entity.Message> UploadMakeThumbnailImageAsync(WCF.Lib.File.Entity.AttachmentsEntity attachmen, int maxHeight, int maxWidth) {
            return base.Channel.UploadMakeThumbnailImageAsync(attachmen, maxHeight, maxWidth);
        }
        
        public WCF.Lib.File.Entity.Message MakeSmailImageByMaxWidthLogo(WCF.Lib.File.Entity.AttachmentsEntity attachmen, int maxWidth, string logo) {
            return base.Channel.MakeSmailImageByMaxWidthLogo(attachmen, maxWidth, logo);
        }
        
        public System.Threading.Tasks.Task<WCF.Lib.File.Entity.Message> MakeSmailImageByMaxWidthLogoAsync(WCF.Lib.File.Entity.AttachmentsEntity attachmen, int maxWidth, string logo) {
            return base.Channel.MakeSmailImageByMaxWidthLogoAsync(attachmen, maxWidth, logo);
        }
        
        public WCF.Lib.File.Entity.Message MakeSmailImageByMaxWidth(WCF.Lib.File.Entity.AttachmentsEntity attachmen, int maxWidth) {
            return base.Channel.MakeSmailImageByMaxWidth(attachmen, maxWidth);
        }
        
        public System.Threading.Tasks.Task<WCF.Lib.File.Entity.Message> MakeSmailImageByMaxWidthAsync(WCF.Lib.File.Entity.AttachmentsEntity attachmen, int maxWidth) {
            return base.Channel.MakeSmailImageByMaxWidthAsync(attachmen, maxWidth);
        }
    }
}
