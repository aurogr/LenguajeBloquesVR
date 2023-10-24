using System.Collections;
using System.Collections.Generic;

public interface IObjectManager
{
    public void StartProgram(Queue<CustomSocketInteractor> sockets);
    public IEnumerator WaitForProgram(Queue<CustomSocketInteractor> sockets);
}
