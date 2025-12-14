Imports System

Module Program
    Sub Main(args As String())


        Dim SufficientRegions As Integer = 0

        Dim Lines() As String = IO.File.ReadAllLines(args(0))

        For i As Integer = 30 To Lines.Length - 1

            Dim Line As String = Lines(i)
            Dim AreaStrings() As String = Line.Substring(0, Line.IndexOf(":")).Split("x")
            Dim Area As Integer = CInt(AreaStrings(0)) * CInt(AreaStrings(1))
            Dim PresentStrings() As String = Line.Substring(Line.IndexOf(":") + 2).Split(" ")
            Dim ShapeCount As Integer = 0
            For Each p As String In PresentStrings
                ShapeCount += CInt(p)
            Next
            Dim ShapeArea As Integer = ShapeCount * 9
            If Area >= ShapeArea Then SufficientRegions += 1

        Next

        Console.WriteLine($"Sufficiently-sized regions: {CStr(SufficientRegions)}")

    End Sub
End Module
