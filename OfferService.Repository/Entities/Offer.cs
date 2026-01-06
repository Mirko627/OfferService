using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OfferService.Shared.enums;

namespace OfferService.Repository.Entities
{
    public class Offer
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public int PropertyId { get; set; }
        [Required]
        public int OfferId { get; set; }
        [Required]
        public int Amount { get; set; }
        public DateOnly CreatedAt { get; set; }
        public DateOnly ExpirateDate { get; set; }
        public OfferStatus Status { get; set; } = OfferStatus.Pending;
    }
}
