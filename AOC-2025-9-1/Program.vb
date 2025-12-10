Imports System
Imports System.Drawing
Imports System.IO

Module Program

    Sub main(args As String())

        Dim Points As New List(Of Point)

        Dim Strings() As String = File.ReadAllLines(args(0))
        For Each Line As String In Strings
            Dim CoordStrings() As String = Line.Split(",")
            Points.Add(New Point(CInt(CoordStrings(0)), CInt(CoordStrings(1))))
        Next

        Dim LargestArea As Long = 0

        For Start As Integer = 0 To Points.Count - 2

            For Other As Integer = 1 To Points.Count - 1

                Dim Point1 As Point = Points.Item(Start)
                Dim Point2 As Point = Points.Item(Other)

                Dim Width As Long = Math.Abs(Point1.X - Point2.X) + 1
                Dim Height As Long = Math.Abs(Point1.Y - Point2.Y) + 1

                Dim Area As Long = Width * Height

                If Area > LargestArea Then LargestArea = Area

            Next

        Next

        Console.WriteLine($"The largest possible area is {CStr(LargestArea)}")

    End Sub

End Module
