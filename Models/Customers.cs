
namespace Storefront.Models//this is the namespace for the Customer model class. It is used to organize code and avoid naming conflicts. The namespace is Storefront.Models, which indicates that this class is part of the Storefront application and belongs to the Models folder.
{
    public class Customer
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = "";

        public string LastName { get; set; } = "";

        public string Email { get; set; } = "";

        public string City { get; set; } = "";
    }
}
