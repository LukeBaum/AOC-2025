Imports System

' This is just the stuff I started for part 1 before realizing how easy this was and there was no part 2...

Module Program
    Sub Main(args As String())

        Dim Shapes As New List(Of Boolean(,))
        Dim Regions As New List(Of (Width As Integer, Height As Integer, PresentAs As Integer, PresentBs As Integer, PresentCs As Integer, PresentDs As Integer, PresentEs As Integer, PresentFs As Integer))

        Dim CurrentShape As New List(Of Boolean())

        For Each Line As String In IO.File.ReadAllLines(args(0))
            If Line.StartsWith(".") Or Line.StartsWith("#") Then
                Dim x(2) As Boolean
                For i As Integer = 0 To 2
                    x(i) = Line.Substring(i, 1) = "#"
                Next
                CurrentShape.Add(x)
            ElseIf Line.Length = 0 Then
                If CurrentShape.Count > 0 Then
                    Dim Shape(2, 2) As Boolean
                    For a As Integer = 0 To 2
                        For b As Integer = 0 To 2
                            Shape(a, b) = CurrentShape.Item(a)(b)
                        Next
                    Next
                    Shapes.Add(Shape)
                    CurrentShape.Clear()
                End If
            ElseIf Shapes.Count > 5 Then

                Dim AreaStrings() As String = Line.Substring(0, Line.IndexOf(":")).Split("x")
                Dim PresentStrings() As String = Line.Substring(Line.IndexOf(":") + 2).Split(" ")
                Regions.Add((CInt(AreaStrings(0)), CInt(AreaStrings(1)), CInt(PresentStrings(0)), CInt(PresentStrings(1)), CInt(PresentStrings(2)), CInt(PresentStrings(3)), CInt(PresentStrings(4)), CInt(PresentStrings(5))))

            End If
        Next

        For Each R In Regions
            Dim Area As Integer = R.Width * R.Height
            Dim x As Integer = ((R.PresentAs * 3) + (R.PresentBs * 3) + (R.PresentCs * 3) + (R.PresentDs * 3) + (R.PresentEs * 3) + (R.PresentFs * 3)) ^ 2
            Console.WriteLine($"{Area} {x}")
        Next

        Console.WriteLine(Shapes.Count)
        Console.WriteLine(Regions.Count)

    End Sub
End Module
