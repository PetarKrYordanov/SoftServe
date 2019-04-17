namespace ImageServe
{
    using AutoMapper;

    using ImageServe.Models;
    using ImageServe.WebModels.Dtos;
    using ImageServe.WebModels.ViewModels;
    using ImageServe.WebModels.BindingModels;
    public class MappingConfiguration : Profile
    {
        public MappingConfiguration()
        {
            CreateMap<Image, ImageDto>();
            CreateMap<User, UserDto>();
            CreateMap<ImageTag, TagDto>();
            CreateMap<Image, ImageViewModel>();
            CreateMap<User, UserViewModel>();
            //CreateMap<Image, SearchImageViewModel>();

            CreateMap<Friendship, FriendshipDto>()
                .ForMember(dto => dto.FriendFullName, map => map.MapFrom(f => f.Friend.GetFullName()))
                .ForMember(dto => dto.FriendAvatar, map => map.MapFrom(f => f.Friend.Avatar));
            CreateMap<User, UserFriendlistViewModel>();
            CreateMap<User, UserEditViewModel>(); //Must be Binding model 
            CreateMap<Image, ImageEditBindingModel>();

        }

    }
}
