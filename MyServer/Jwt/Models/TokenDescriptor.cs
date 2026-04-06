using System.Collections.Generic;

namespace MyServer.Jwt.Models;

public record TokenDescriptor(List<ClaimsType>Claims,byte[] Key);