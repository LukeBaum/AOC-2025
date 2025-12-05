Imports System
Imports System.IO
Imports System.Runtime.CompilerServices

Module Program
    Sub Main(args As String())

        ' Once again, we'll dispatch with that fancy "error handling".

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

        FreshRanges.Sort(Function(x, y) x.First.CompareTo(y.First))
        For Each Range As FreshRange In FreshRanges
            Console.WriteLine($"{CStr(Range.First)}-{CStr(Range.Last)}")
        Next

        Dim MergesPerformed As Integer = -1
        Do Until MergesPerformed = 0
            MergesPerformed = FreshRanges.MergeRanges()
            Console.WriteLine(CStr(MergesPerformed))
        Loop

        'FreshRanges.MergeRanges()

        FreshRanges.Sort(Function(x, y) x.First.CompareTo(y.First))
        For Each s As FreshRange In FreshRanges
            Console.WriteLine($"{CStr(s.First)}-{CStr(s.Last)}")
        Next

        Dim TotalIds As Long = 0
        For Each Range As FreshRange In FreshRanges
            TotalIds += (Range.Last - Range.First + 1)
        Next

        Console.WriteLine($"Total fresh ids: {CStr(TotalIds)}")

    End Sub

    Public Class FreshRange
        Public First As Long
        Public Last As Long
        Sub New(ByVal First As Long, ByVal Last As Long)
            Me.First = First
            Me.Last = Last
        End Sub
    End Class

    <Extension()> Public Function MergeRanges(ByRef FreshRanges As List(Of FreshRange)) As Integer

        Dim MergesPerformed As Integer = 0

        For i As Integer = FreshRanges.Count - 1 To 1 Step -1
            Dim x As FreshRange = FreshRanges.Item(i - 1)
            Dim y As FreshRange = FreshRanges.Item(i)
            If y.First <= x.Last + 1 Then
                x.Last = y.Last
                FreshRanges.Remove(y)
                MergesPerformed += 1
            End If
        Next

        Return MergesPerformed

    End Function

End Module
