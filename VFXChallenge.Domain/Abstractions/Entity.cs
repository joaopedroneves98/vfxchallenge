namespace VFXChallenge.Domain.Abstractions
{
    public abstract class Entity
    {
        protected Entity(Guid id)
        {
            Id = id;
        }

        protected Entity()
        {

        }

        public Guid Id { get; init; }
    
        public DateTime Created { get; set; }
    
        public DateTime Updated { get; set; }
        
    }
}