namespace pharmacy_management.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(string email)
            : base($"User with email '{email}' not found.")
        { }
    }

    public class InvalidCredentialsException : Exception
    {
        public InvalidCredentialsException()
            : base("Invalid email or password.")
        { }
    }

    public class UserAlreadyExistsException : Exception
    {
        public UserAlreadyExistsException(string email)
            : base($"User with email '{email}' already exists.")
        { }
    }

    public class PasswordsDoNotMatchException : Exception
    {
        public PasswordsDoNotMatchException()
            : base("Passwords do not match.")
        { }
    }
}
