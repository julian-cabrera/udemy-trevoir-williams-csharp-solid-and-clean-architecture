using AutoMapper;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.DTOs.LeaveType;
using HR.LeaveManagement.Application.Exceptions;
using HR.LeaveManagement.Application.Features.LeaveTypes.Handlers.Commands;
using HR.LeaveManagement.Application.Features.LeaveTypes.Requests.Commands;
using HR.LeaveManagement.Application.Profiles;
using HR.LeaveManagement.Application.Responses;
using HR.LeaveManagement.Application.UnitTests.Mock;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using Moq;
using Shouldly;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HR.LeaveManagement.Application.UnitTests.LeaveTypes.Commands
{
    public class CreateLeaveTypeCommandHandlerTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly CreateLeaveTypeDto _leaveTypeDTO;
        private readonly CreateLeaveTypeCommandHandler _handler;
        public CreateLeaveTypeCommandHandlerTests()
        {
            _mockUow = MockUnitOfWork.GetUnitOfWork();

            var mapperConfig = new MapperConfiguration(x =>
            {
                x.AddProfile<MappingProfile>();
            });

            _mapper = mapperConfig.CreateMapper();

            _leaveTypeDTO = new CreateLeaveTypeDto
            {
                DefaultDays = 15,
                Name = "Test DTO"
            };

            _handler = new CreateLeaveTypeCommandHandler(_mockUow.Object, _mapper);
        }

        [Fact]
        public async Task Create_Valid_Leave_Type()
        {
            var result = await _handler.Handle(new CreateLeaveTypeCommand() { LeaveTypeDTO = _leaveTypeDTO }, CancellationToken.None);
            var leaveTypes = await _mockUow.Object.LeaveTypeRepository.GetAll();
            
            result.ShouldBeOfType<BaseCommandResponse>();
            leaveTypes.Count.ShouldBe(3);
        }
        [Fact]
        public async Task Create_Invalid_Leave_Type()
        {
            _leaveTypeDTO.DefaultDays = -1;

            //ValidationException ex = await Should.ThrowAsync<ValidationException>(
            //    async () => await _handler.Handle(new CreateLeaveTypeCommand() { LeaveTypeDTO = _leaveTypeDTO }, CancellationToken.None));
            var result = await _handler.Handle(new CreateLeaveTypeCommand() { LeaveTypeDTO = _leaveTypeDTO }, CancellationToken.None);

            var leaveTypes = await _mockUow.Object.LeaveTypeRepository.GetAll();

            leaveTypes.Count.ShouldBe(2);
            result.ShouldBeOfType<BaseCommandResponse>();
        }
    }
}
