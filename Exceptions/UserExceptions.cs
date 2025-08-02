namespace pharmacy_management.Exceptions
{
    public class UserNotFoundException(string email) : Exception($"User with email '{email}' not found.")
    {
    }

    public class InvalidCredentialsException : Exception
    {
        public InvalidCredentialsException()
            : base("Invalid email or password.")
        { }
    }

    public class UserAlreadyExistsException(string email) : Exception($"User with email '{email}' already exists.")
    {
    }

    public class PasswordsDoNotMatchException : Exception
    {
        public PasswordsDoNotMatchException()
            : base("Passwords do not match.")
        { }
    }

    // product categories exceptions

    public class CategoryNotFoundException(Guid id) : Exception($"Category with id '{id}' not found.")
    {
    }

    public class CategoryAlreadyExistsException(string name) : Exception($"Category with name '{name}' already exists.")
    {
    }
}
