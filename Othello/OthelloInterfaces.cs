namespace Othello
{

    /// <summary>
    /// The abstract aggregate interface for creating iterator for OthelloBoard
    /// </summary>
    public interface IOthelloAbstractBoardCollection
    {
        OthelloBoardIterator CreateIterator();
    }


    /// <summary>
    /// An abstract class capable of cloning itself. Can be used in conjunction with Manager type classes that register and creates (which is called the Prototype pattern)
    /// This is extremely useful in cases when:
    /// 1# there are thousands of type of classes, but when there are too many types, we cannot instantiate them easily and maintain source files
    /// 2# When it is difficult to instantiate classes, in an application where there is a lot of members and variables that are owned by the class but we still need to copy them
    /// 3# when decoupling is required between the class in an assembly and the program to instantiate it, instead of using class names, we can use string names to instantiate them.
    /// TODO: should be extended to add "use" methods for extensibility and less dependence on OthelloState from other classes such as OthelloAI
    /// http://www.rarestyle.net/main/patterns/prototype.aspx
    /// </summary>
    public interface IOthelloPrototypeState
    {
        IOthelloPrototypeState Clone();
    }
}
