using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartPark.CQRS.Commands.Slot;
using SmartPark.CQRS.Queries.Slot;
using SmartPark.Data.Contexts;
using SmartPark.Dtos.Slot;

namespace SmartPark.CQRS.Handlers.Slot
{
    public class SlotHandlers :
       IRequestHandler<CreateSlotCommand, SlotResponseDto>,
       IRequestHandler<UpdateSlotCommand, SlotResponseDto>,
       IRequestHandler<DeleteSlotCommand, bool>,
       IRequestHandler<GetSlotByIdQuery, SlotResponseDto?>,
       IRequestHandler<GetAllSlotsQuery, IEnumerable<SlotResponseDto>>
    {
        private readonly ParkingDbContext _dbContext;

        public SlotHandlers(ParkingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<SlotResponseDto> Handle(CreateSlotCommand request, CancellationToken cancellationToken)
        {
            var entity = new Models.Slot
            {
                Id = Guid.NewGuid(),
                LocationId = request.Dto.LocationId,
                SlotType = request.Dto.SlotType,
                IsAvailable = request.Dto.IsAvailable ?? true
            };

            _dbContext.Slots.Add(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new SlotResponseDto
            {
                Id = entity.Id,
                LocationId = entity.LocationId,
                SlotType = entity.SlotType,
                IsAvailable = entity.IsAvailable
            };
        }

        public async Task<SlotResponseDto> Handle(UpdateSlotCommand request, CancellationToken cancellationToken)
        {
            var entity = await _dbContext.Slots.FindAsync(new object[] { request.Id }, cancellationToken);
            if (entity == null) throw new KeyNotFoundException("Slot not found");

            entity.SlotType = request.Dto.SlotType ?? entity.SlotType;
            entity.IsAvailable = request.Dto.IsAvailable ?? entity.IsAvailable;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return new SlotResponseDto
            {
                Id = entity.Id,
                LocationId = entity.LocationId,
                SlotType = entity.SlotType,
                IsAvailable = entity.IsAvailable
            };
        }

        public async Task<bool> Handle(DeleteSlotCommand request, CancellationToken cancellationToken)
        {
            var entity = await _dbContext.Slots.FindAsync(new object[] { request.Id }, cancellationToken);
            if (entity == null) return false;

            _dbContext.Slots.Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<SlotResponseDto?> Handle(GetSlotByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _dbContext.Slots.AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

            if (entity == null) return null;

            return new SlotResponseDto
            {
                Id = entity.Id,
                LocationId = entity.LocationId,
                SlotType = entity.SlotType,
                IsAvailable = entity.IsAvailable
            };
        }

        public async Task<IEnumerable<SlotResponseDto>> Handle(GetAllSlotsQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Slots.AsNoTracking()
                .Select(s => new SlotResponseDto
                {
                    Id = s.Id,
                    LocationId = s.LocationId,
                    SlotType = s.SlotType,
                    IsAvailable = s.IsAvailable
                })
                .ToListAsync(cancellationToken);
        }
    }
}
