
public class Message 
{

}


public class ErrorMsg : Message
{
    public string msg;
    public ErrorMsg(string msg)
    {
        this.msg = msg;
    }
}