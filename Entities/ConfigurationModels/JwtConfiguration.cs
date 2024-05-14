namespace Entities.ConfigurationModels
{
    public class JwtConfiguration
    {
        public string Section { get; set; } = "JwtSettings";

        private string? _validIssuer;
        private string? _validAudience;
        private string? _expires;
        private string? _rExpires;

        public string? ValidIssuer
        {
            get => _validIssuer;
            set => _validIssuer = value ?? _validIssuer;
        }

        public string? ValidAudience
        {
            get => _validAudience;
            set => _validAudience = value ?? _validAudience;
        }

        public string? Expires
        {
            get => _expires;
            set => _expires = value ?? _expires;
        }

        public string? rExpires
        {
            get => _rExpires;
            set => _rExpires = value ?? _rExpires;
        }
    }
}
