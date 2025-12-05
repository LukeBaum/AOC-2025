Imports System
Imports System.IO
Imports System.Runtime.CompilerServices

Module Program
    Sub Main(args As String())

        ' Once again, we'll dispatch with that fancy "error handling".

        Dim InventoryStrings() As String = File.ReadAllLines(args(0))

        Dim FreshRanges As New List(Of FreshRange)
        Dim Position As Integer = 0
        Dim CurrentLine As String = InventoryStrings(Position)
        Do Until CurrentLine = ""
            Dim Range() As String = CurrentLine.Split("-")
            FreshRanges.AddCorrectly(New FreshRange(CLng(Range(0)), CLng(Range(1))))
            Position += 1
            CurrentLine = InventoryStrings(Position)
        Loop

        Dim TotalIds As Long = 0
        For Each Range As FreshRange In FreshRanges
            TotalIds += (Range.Last - Range.First + 1)
        Next

        Console.WriteLine($"Total fresh ids: {CStr(TotalIds)}")

    End Sub

    Public Structure FreshRange
        Public First As Long
        Public Last As Long
        Sub New(ByVal First As Long, ByVal Last As Long)
            Me.First = First
            Me.Last = Last
        End Sub
    End Structure

    <Extension()> Public Sub AddCorrectly(ByRef FreshRanges As List(Of FreshRange), ByVal Range As FreshRange)

        Dim TouchingRanges As New List(Of FreshRange)

        ' I could have just copy pasta'd this from PCP Auto Count, if this was Python. Felt familiar.

        For Each ExistingRange As FreshRange In FreshRanges
            If Range.First <= ExistingRange.Last And ExistingRange.First <= Range.Last Then
                TouchingRanges.Add(ExistingRange)
            End If
        Next

        If TouchingRanges.Count = 0 Then
            FreshRanges.Add(Range)
            Exit Sub
        End If

        Dim NewRange As New FreshRange(Range.First, Range.Last)

        For Each TouchingRange As FreshRange In TouchingRanges
            If TouchingRange.First < NewRange.First Then NewRange.First = TouchingRange.First
            If TouchingRange.Last > NewRange.Last Then NewRange.Last = TouchingRange.Last
            FreshRanges.Remove(TouchingRange)
        Next

        FreshRanges.Add(NewRange)

    End Sub

End Module
