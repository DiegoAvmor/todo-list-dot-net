using AutoMapper;
using TodoListApi.Models.Data;
using TodoListApi.Models.Data.DTO;

namespace TodoListApi.Config
{
    public class MappingConfig: Profile
    {
        public MappingConfig(){
            CreateMap<User,LoginRequestDto>().ReverseMap();
            CreateMap<User,RegistrationRequestDto>().ReverseMap();
            CreateMap<User,UserDto>().ReverseMap();

            CreateMap<TodoTask,TodoTaskDto>().ReverseMap();
            CreateMap<TodoTask,TodoTaskRequestDto>().ReverseMap();
            CreateMap<TodoTask,TodoTaskResponseDto>()
            .ForMember(d => d.user, opt => opt.MapFrom(src => src.User))
            .ReverseMap();
        }
        
    }
}