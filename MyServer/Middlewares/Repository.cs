using System.Collections.Generic;

namespace MyServer.Middlewares;

public class Repository
{
    private static Repository? _repository ;
    private Repository()
    {
        
    }

    public string Body { get; set; }
    public List<string> Header { get; set; } = [];
    public string Method { get; set; }
    public string Path  { get; set; }
    
    public static Repository Create()
    {
        if (_repository is null)
            _repository = new Repository();
        
        return _repository;
    }
}