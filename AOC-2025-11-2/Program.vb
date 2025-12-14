' I'm proud to have solved this one without AI. I did have to learn about DAGs and Memoization though.

Module Program
    Sub Main(args As String())

        Dim Shortcuts As New Dictionary(Of String, (TotalPaths As Long, TotalDacOnlyPaths As Long, TotalFftOnlyPaths As Long, TotalDacFftPaths As Long))

        Dim Stops As New Dictionary(Of String, String())

        For Each Line As String In IO.File.ReadAllLines(args(0))
            Dim Fields() As String = Line.Split(" ")
            Dim Destinations(Fields.Length - 2) As String
            For i As Integer = 1 To Fields.Length - 1
                Destinations(i - 1) = Fields(i)
            Next
            Stops.Add(Fields(0).Substring(0, 3), Destinations)
        Next

        Dim CanFindMore As Boolean = True

        Do While CanFindMore = True

            Dim OriginalCount As Integer = Shortcuts.Count

            For Each s As String In Stops.Keys
                PathsFromXToOut(s, Stops, Shortcuts)
            Next

            If OriginalCount = Shortcuts.Count Then
                CanFindMore = False
            Else
                Console.WriteLine($"Added {Shortcuts.Count - OriginalCount} shortcuts")
            End If

        Loop

        Console.WriteLine($"Total relevant paths from svr to out: {CStr(Shortcuts("svr").TotalDacFftPaths)}")

    End Sub

    Public Sub PathsFromXToOut(ByVal Start As String, ByVal Stops As Dictionary(Of String, String()), ByRef Shortcuts As Dictionary(Of String, (TotalPaths As Long, TotalDacPaths As Long, TotalFftPaths As Long, TotalDacFftPaths As Long)))

        If Shortcuts.Keys.Contains(Start) Then Exit Sub

        Dim PathsToOut As Long = 0
        Dim DacPathsToOut As Long = 0
        Dim FftPathsToOut As Long = 0
        Dim DacAndFftPathsToOut As Long = 0

        Dim Paths As New List(Of (CurrentStop As String, TotalStops As Long, Dac As Boolean, Fft As Boolean))
        Paths.Add((Start, 0, Start = "dac", Start = "fft"))

        Dim incr As Integer = 0
        Do While Paths.Count > 0

            incr += 1

            If Shortcuts.Keys.Contains(Paths(0).CurrentStop) Then

                Dim Shortcut = Shortcuts(Paths(0).CurrentStop)

                PathsToOut += 1 * Shortcut.TotalPaths

                If Paths(0).Dac = True And Paths(0).Fft = True Then
                    DacAndFftPathsToOut += 1 * Shortcut.TotalPaths
                    DacPathsToOut += 1 * Shortcut.TotalPaths
                    FftPathsToOut += 1 * Shortcut.TotalPaths
                ElseIf Paths(0).Dac = True And Paths(0).Fft = False Then
                    DacAndFftPathsToOut += 1 * Shortcut.TotalFftPaths
                    DacPathsToOut += 1 * Shortcut.TotalPaths
                    FftPathsToOut += 1 * Shortcut.TotalFftPaths
                ElseIf Paths(0).Dac = False And Paths(0).Fft = True Then
                    DacAndFftPathsToOut += 1 * Shortcut.TotalDacPaths
                    DacPathsToOut += 1 * Shortcut.TotalDacPaths
                    FftPathsToOut += 1 * Shortcut.TotalPaths
                Else
                    DacAndFftPathsToOut += 1 * Shortcut.TotalDacFftPaths
                    DacPathsToOut += 1 * Shortcut.TotalDacPaths
                    FftPathsToOut += 1 * Shortcut.TotalFftPaths
                End If

            ElseIf Paths(0).CurrentStop = "out" Then
                PathsToOut += 1
                If Paths(0).Dac = True And Paths(0).Fft = True Then DacAndFftPathsToOut += 1
                If Paths(0).Dac = True Then DacPathsToOut += 1
                If Paths(0).Fft = True Then FftPathsToOut += 1
            Else
                If Paths(0).CurrentStop = "dac" Then Paths(0) = (Paths(0).CurrentStop, Paths(0).TotalStops, True, Paths(0).Fft)
                If Paths(0).CurrentStop = "fft" Then Paths(0) = (Paths(0).CurrentStop, Paths(0).TotalStops, Paths(0).Dac, True)

                For Each NextStop As String In Stops(Paths(0).CurrentStop)
                    Paths.Add((NextStop, Paths(0).TotalStops + 1, Paths(0).Dac, Paths(0).Fft))
                Next
            End If

            Paths.RemoveAt(0)

            If incr > 500 Then
                PathsToOut = -2
                Exit Do
            End If

        Loop

        If PathsToOut > -1 Then
            Shortcuts.Add(Start, (PathsToOut, DacPathsToOut, FftPathsToOut, DacAndFftPathsToOut))
        End If

    End Sub

End Module
