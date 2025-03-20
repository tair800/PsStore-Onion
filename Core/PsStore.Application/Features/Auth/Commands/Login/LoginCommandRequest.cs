using MediatR;
using System.ComponentModel;

namespace PsStore.Application.Features.Auth.Commands.Login
{
    public class LoginCommandRequest : IRequest<Result<LoginCommandResponse>>
    {
        [DefaultValue("tahir@mail.ru")]
        public string Email { get; set; }
        [DefaultValue("12345@Tt")]
        public string Password { get; set; }
    }
}
