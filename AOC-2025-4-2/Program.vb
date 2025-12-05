Imports System
Imports System.IO
Imports System.Drawing
Imports System.Runtime.CompilerServices

Module Program
    Sub Main(args As String())

        ' Validate the correct number of arguments are present
        If args.Length = 0 Then Console.WriteLine("No rolls file provided")
        If args.Length > 1 Then Console.WriteLine("Too many arguments")
        If args.Length <> 1 Then Exit Sub

        ' Make sure the path to the rolls file exists
        Dim RollsFilePath As String = args(0)
        If Not File.Exists(RollsFilePath) Then
            Console.WriteLine($"Rolls file doesn't exist: {RollsFilePath}")
            Exit Sub
        End If

        ' Make sure we can actually read the file
        Dim RollsStrings() As String = Nothing
        Try
            RollsStrings = File.ReadAllLines(RollsFilePath)
        Catch ex As Exception
            Console.WriteLine($"Error reading rolls file: {ex.Message}")
            Exit Sub
        End Try

        ' Make sure the file wasn't empty
        If RollsStrings.Count < 1 Then
            Console.WriteLine("No rolls in the file")
            Exit Sub
        End If

        ' Parse the file
        Dim PaperSlots As New List(Of PaperSlot)
        PaperSlots.ImportSlotsFromStrings(RollsStrings)

        ' Remove accessible rolls in waves until we can't remove anymore
        ' This is terribly unperformant for laughs
        ' Seriously just use an an array next time
        Dim RemovedRolls As Integer = 0
        Dim PreviouslyRemovedRolls = -1
        Do Until PreviouslyRemovedRolls = 0
            PreviouslyRemovedRolls = PaperSlots.RemoveAccessibleRolls()
            RemovedRolls += PreviouslyRemovedRolls
            Console.WriteLine($"Completed one wave of {CStr(PreviouslyRemovedRolls)} removals")
        Loop

        Console.WriteLine($"Total rolls removed: {CStr(RemovedRolls)}")

    End Sub

    Public Class PaperSlot

        Public Property Occupied As Boolean = False
        Public Property Location As Point = New Point(-1, -1)

        Sub New(Optional ByVal IsOccupied As Boolean = False, Optional ByVal X As Integer = -1, Optional ByVal Y As Integer = -1)
            Me.Occupied = IsOccupied
            Me.Location = New Point(X, Y)
        End Sub

    End Class

    <Extension()> Public Sub ImportSlotsFromStrings(ByRef PaperSlots As List(Of PaperSlot), ByRef RollsStrings() As String)

        For Y As Integer = 0 To RollsStrings.Length - 1
            Dim CurrentRow As String = RollsStrings(Y)
            For X As Integer = 0 To CurrentRow.Length - 1
                PaperSlots.Add(New PaperSlot(CurrentRow.Substring(X, 1) = "@", X, Y))
            Next
        Next

    End Sub

    <Extension()> Public Function GetSlotAtLocation(ByRef PaperSlots As List(Of PaperSlot), ByVal X As Integer, ByVal Y As Integer) As PaperSlot
        For Each Slot As PaperSlot In PaperSlots
            If Slot.Location.X = X And Slot.Location.Y = Y Then
                Return Slot
                Exit For
            End If
        Next
        Return Nothing
    End Function

    <Extension()> Public Function SlotContainsAccessiblePaperRoll(ByRef PaperSlots As List(Of PaperSlot), ByVal X As Integer, ByVal Y As Integer) As Boolean

        Dim SelectedPaperSlot As PaperSlot = PaperSlots.GetSlotAtLocation(X, Y)

        If SelectedPaperSlot Is Nothing Then Return False
        If SelectedPaperSlot.Occupied = False Then Return False

        Dim AdjacentRolls As Integer = 0

        For a As Integer = -1 To 1
            For b As Integer = -1 To 1
                If a = 0 And b = 0 Then Continue For
                Dim AdjacentSlot As PaperSlot = PaperSlots.GetSlotAtLocation(X + a, Y + b)
                If AdjacentSlot Is Nothing Then Continue For
                If AdjacentSlot.Occupied Then AdjacentRolls += 1
                If AdjacentRolls >= 4 Then Return False
            Next
        Next

        Return True

    End Function

    <Extension()> Public Function RemoveAccessibleRolls(ByRef PaperSlots As List(Of PaperSlot)) As Integer

        ' Locates all accessible rolls, removes them, and returns the number of removed rolls.
        Dim AccessibleRolls As New List(Of Point)

        For Each Slot As PaperSlot In PaperSlots
            If PaperSlots.SlotContainsAccessiblePaperRoll(Slot.Location.X, Slot.Location.Y) Then AccessibleRolls.Add(Slot.Location)
        Next

        Dim AccessibleRollCount As Integer = AccessibleRolls.Count

        For Each p As Point In AccessibleRolls
            PaperSlots.GetSlotAtLocation(p.X, p.Y).Occupied = False
        Next

        Return AccessibleRollCount

    End Function

End Module
