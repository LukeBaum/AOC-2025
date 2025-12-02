Imports System
Imports System.IO
Imports System.Runtime.CompilerServices

Module Program
    Sub Main(args As String())

        ' Validate the correct number of arguments are present
        If args.Length = 0 Then Console.WriteLine("No id file provided")
        If args.Length > 1 Then Console.WriteLine("Too many arguments")
        If args.Length <> 1 Then Exit Sub

        ' Make sure the path to the ids file exists
        Dim IdFilePath As String = args(0)
        If Not File.Exists(IdFilePath) Then
            Console.WriteLine($"Id file doesn't exist: {IdFilePath}")
            Exit Sub
        End If

        ' Make sure we can actually read the file
        Dim IdsString() As String = Nothing
        Try
            IdsString = File.ReadAllText(IdFilePath).Split(",")
        Catch ex As Exception
            Console.WriteLine($"Error reading id file: {ex.Message}")
            Exit Sub
        End Try

        ' Make sure the file wasn't empty
        If IdsString.Count < 1 Then
            Console.WriteLine("No ids in the file")
            Exit Sub
        End If

        ' Find the bad ids.
        FindBadIds(IdsString.ToIntegerRanges())

    End Sub

    Public Sub FindBadIds(ByRef IdRanges As List(Of IntegerRange))

        Dim BadSum As Long = 0

        For Each IdRange As IntegerRange In IdRanges
            For i As Long = IdRange.First To IdRange.Last
                ' Get a load of this tomfoolery
                Dim istr As String = i.ToString()
                For n As Integer = 1 To istr.Length - 1
                    Dim Clone As String = ""
                    Do While Clone.Length < istr.Length
                        Clone &= istr.Substring(0, n)
                    Loop
                    If CLng(istr) = CLng(Clone) Then
                        BadSum += i
                        Exit For
                    End If
                Next
            Next
        Next

        Console.WriteLine($"The sum of bad ids is {CStr(BadSum)}")

    End Sub

    <Extension()> Public Function ToIntegerRanges(ByVal Strings() As String)

        Dim Ranges As New List(Of IntegerRange)

        For Each RangeString In Strings
            Dim NumStrings() As String = RangeString.Split("-")
            Dim NewRange As New IntegerRange(CLng(NumStrings(0)), CLng(NumStrings(1)))
            Ranges.Add(NewRange)
        Next

        Return Ranges

    End Function

    ' Why use a class when you can use a...
    Public Structure IntegerRange ' Actually long integers but who's counting?
        Public First As Long
        Public Last As Long
        Sub New(ByVal First As Long, ByVal Last As Long)
            Me.First = First
            Me.Last = Last
        End Sub
    End Structure

End Module
