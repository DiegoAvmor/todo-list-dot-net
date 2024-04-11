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
        }
        
    }
}