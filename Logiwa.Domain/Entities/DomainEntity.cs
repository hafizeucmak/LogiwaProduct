using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace Logiwa.Domain.Entities
{
    public interface IDomainEntity
    {
        void Update();

        void Delete();

        bool Equals(DomainEntity entity);
    }

    public abstract class DomainEntity : IDomainEntity
    {
        private readonly DomainEntityValidator _validator = new();

        public DomainEntity()
        {
            CreatedAt = DateTime.UtcNow;
            _validator.ValidateAndThrow(this);

        }

        [Key]
        public int Id { get; protected set; }

        public DateTime CreatedAt { get; protected set; }

        public DateTime? UpdatedAt { get; protected set; }

        public DateTime? DeletedAt { get; protected set; }

        public virtual void Update()
        {
            UpdatedAt = DateTime.UtcNow;
        }

        public virtual void Delete()
        {
            DeletedAt = DateTime.UtcNow;
        }

        public bool Equals(DomainEntity entity)
        {
            if (entity == null)
                return false;

            return Id == entity.Id;
        }
    }

    public class DomainEntityValidator : AbstractValidator<DomainEntity>
    {
        public DomainEntityValidator()
        {
            RuleFor(x => x.UpdatedAt).NotEqual(default(DateTime))
                                     .When(x => x.UpdatedAt.HasValue);
            RuleFor(x => x.DeletedAt).NotEqual(default(DateTime))
                                     .When(x => x.DeletedAt.HasValue);
        }
    }
}
