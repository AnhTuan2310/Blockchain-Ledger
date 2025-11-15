using Application.DTOs.Snapshot;
using Domain.Entities;

namespace Application.Mappings {
    public static class SnapshotMappingExtensions {
        public static SnapshotDTO ToSnapshotDTO(this MemoryRecord record) =>
            new SnapshotDTO {
                Id = record.Id,
                Description = record.Description,
                CreatedAt = record.CreatedAt
            };
    }
}
