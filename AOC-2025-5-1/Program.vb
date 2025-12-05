Imports System
Imports System.IO

Module Program

    Sub Main(args As String())

        ' You know what? Let's just get rid of all the safety to save time.

        Dim InventoryStrings() As String = File.ReadAllLines(args(0))

        Dim Position As Integer = 0
        Dim CurrentLine As String = "na"

        Dim FreshRanges As New List(Of FreshRange)

        CurrentLine = InventoryStrings(Position)
        Do Until CurrentLine = ""
            Dim Range() As String = CurrentLine.Split("-")
            FreshRanges.Add(New FreshRange(CLng(Range(0)), CLng(Range(1))))
            Position += 1
            CurrentLine = InventoryStrings(Position)
        Loop

        Position += 1
        Dim FreshIngredientCount As Integer = 0

        Do While Position < InventoryStrings.Length
            Dim TestIngredient As Long = CLng(InventoryStrings(Position))
            For Each Range As FreshRange In FreshRanges
                If TestIngredient >= Range.First And TestIngredient <= Range.Last Then
                    FreshIngredientCount += 1
                    Exit For
                End If
            Next
            Position += 1
        Loop

        Console.WriteLine($"Total fresh ingredients: {CStr(FreshIngredientCount)}")

    End Sub

    Public Structure FreshRange
        Public First As Long
        Public Last As Long
        Sub New(ByVal First As Long, ByVal Last As Long)
            Me.First = First
            Me.Last = Last
        End Sub
    End Structure

End Module
