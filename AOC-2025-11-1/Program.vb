Imports System
Imports System.IO

Module Program
    Sub Main(args As String())

        Dim Stops As New Dictionary(Of String, String())

        For Each Line As String In File.ReadAllLines(args(0))
            Dim Fields() As String = Line.Split(" ")
            Dim Destinations(Fields.Length - 2) As String
            For i As Integer = 1 To Fields.Length - 1
                Destinations(i - 1) = Fields(i)
            Next
            Stops.Add(Fields(0).Substring(0, 3), Destinations)
        Next

        Console.WriteLine($"Total number of paths from you to out: {CStr(PathsFromYouToOut(Stops))}")

    End Sub

    Public Function PathsFromYouToOut(ByVal Stops As Dictionary(Of String, String())) As Integer

        Dim PathsToOut As Integer = 0

        Dim Paths As New List(Of (CurrentStop As String, TotalStops As Integer))
        Paths.Add(("you", 0))

        Do While Paths.Count > 0

            If Paths(0).CurrentStop = "out" Then
                PathsToOut += 1
            Else
                For Each NextStop As String In Stops(Paths(0).CurrentStop)
                    Paths.Add((NextStop, Paths(0).TotalStops + 1))
                Next
            End If

            Paths.RemoveAt(0)

        Loop

        Return PathsToOut

    End Function

End Module
