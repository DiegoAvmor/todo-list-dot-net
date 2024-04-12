using AutoMapper;
using TodoListApi.Models.Data;
using TodoListApi.Models.Data.DTO;

namespace TodoListApi.Config
{
    public class MappingConfig: Profile
    {
        public MappingConfig(){
            CreateMap<User,LoginRequestDTO>().ReverseMap();
            CreateMap<User,RegistrationRequestDTO>().ReverseMap();
            CreateMap<User,UserDTO>().ReverseMap();

            CreateMap<TodoTask,TodoTaskDTO>().ReverseMap();
            CreateMap<TodoTask,TodoTaskRequestDTO>().ReverseMap();
            CreateMap<TodoTask,TodoTaskResponseDTO>()
            .ForMember(d => d.user, opt => opt.MapFrom(src => src.User))
            .ReverseMap();
        }
        
    }
}