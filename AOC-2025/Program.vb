Imports System
Imports System.IO
Imports System.Runtime.CompilerServices

Module Program

    Sub Main(args As String())

        ' Validate the correct number of arguments are present
        If args.Length = 0 Then Console.WriteLine("No combination file provided")
        If args.Length > 1 Then Console.WriteLine("Too many arguments")
        If args.Length <> 1 Then Exit Sub

        ' Make sure the path to the combination file exists
        Dim CombinationFilePath As String = args(0)
        If Not File.Exists(CombinationFilePath) Then
            Console.WriteLine($"Combination file doesn't exist: {CombinationFilePath}")
            Exit Sub
        End If

        ' Make sure we can actually read the file
        Dim CombinationStrings As List(Of String) = Nothing
        Try
            CombinationStrings = File.ReadAllLines(CombinationFilePath).ToList()
        Catch ex As Exception
            Console.WriteLine($"Error reading combination file: {ex.Message}")
            Exit Sub
        End Try

        ' Make sure the file wasn't empty
        If CombinationStrings.Count < 1 Then
            Console.WriteLine("No combinations in the file")
            Exit Sub
        End If

        ' Convert the lines from the file to directional integers
        Dim Combination As List(Of Integer)
        Try
            Combination = CombinationStrings.ToIntegers()
        Catch ex As Exception
            Console.WriteLine($"Error parsing combinations: {ex.Message}")
            Exit Sub
        End Try

        ' Determine the password by twisting the dial according to the instructions
        Try
            Console.WriteLine($"The password is: {CStr(FindPassword(Combination))}")
        Catch ex As Exception
            Console.WriteLine($"Error finding password: {ex.Message}")
        End Try

    End Sub

    <Extension()> Public Function ToIntegers(ByVal Strings As List(Of String))

        Dim Integers As New List(Of Integer)
        For Each s As String In Strings
            Dim Direction As String = s.Substring(0, 1)
            Dim DistanceString As String = s.Substring(1)
            If DistanceString.Length > 2 Then DistanceString = DistanceString.Substring(DistanceString.Length - 2)
            Dim Distance As Integer = CInt(DistanceString)
            If Not {"L", "R"}.Contains(Direction) Then Throw New InvalidDataException($"Invalid direction specifier: {Direction}")
            If Direction = "L" Then Distance *= -1
            Integers.Add(Distance)
        Next

        Return Integers

    End Function

    Public Function FindPassword(ByRef Directions As List(Of Integer))

        Dim Position As Integer = 50
        Dim TimesAtZero As Integer = 0

        For Each Instruction As Integer In Directions

            Position += Instruction
            If Position > 99 Then
                Position -= 100
            ElseIf Position < 0 Then
                Position += 100
            End If

            If Position = 0 Then TimesAtZero += 1

        Next

        Return TimesAtZero

    End Function

End Module
