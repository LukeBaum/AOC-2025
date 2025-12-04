Imports System
Imports System.IO
Imports System.Reflection.PortableExecutable

Module Program
    Sub Main(args As String())

        ' Validate the correct number of arguments are present
        If args.Length = 0 Then Console.WriteLine("No battery file provided")
        If args.Length > 1 Then Console.WriteLine("Too many arguments")
        If args.Length <> 1 Then Exit Sub

        ' Make sure the path to the battery file exists
        Dim BatteryFilePath As String = args(0)
        If Not File.Exists(BatteryFilePath) Then
            Console.WriteLine($"Battery file doesn't exist: {BatteryFilePath}")
            Exit Sub
        End If

        ' Make sure we can actually read the file
        Dim BankStrings() As String = Nothing
        Try
            BankStrings = File.ReadAllLines(BatteryFilePath)
        Catch ex As Exception
            Console.WriteLine($"Error reading battery file: {ex.Message}")
            Exit Sub
        End Try

        ' Make sure the file wasn't empty
        If BankStrings.Count < 1 Then
            Console.WriteLine("No battery banks in the file")
            Exit Sub
        End If

        Console.WriteLine($"Total joltage output: {FindSumOfMaximumJoltages(BankStrings)}")

    End Sub

    Private Function FindSumOfMaximumJoltages(ByRef BankStrings() As String) As Long

        Dim JoltageSum As Long = 0

        For Each BankString As String In BankStrings
            Dim m As Long = FindMaximumJoltage(ConvertBankStringToJoltageList(BankString))
            JoltageSum += m
            Console.WriteLine(CStr(m))
        Next

        Return JoltageSum

    End Function

    Private Function FindMaximumJoltage(ByRef JoltageList As List(Of Integer)) As Long

        Dim BatteryList As New List(Of Integer)

        Dim Position As Integer = 0

        For i As Integer = 11 To 0 Step -1
            BatteryList.Add(JoltageList.GetRange(Position, (JoltageList.Count - i) - Position).Max)
            Position = JoltageList.IndexOf(BatteryList.Last, Position) + 1
        Next

        Dim BatteryString As String = ""
        For Each Battery As Integer In BatteryList
            BatteryString &= CStr(Battery)
        Next

        Return CLng(BatteryString)

    End Function

    Private Function ConvertBankStringToJoltageList(ByRef BankString As String) As List(Of Integer)
        Dim Joltages As New List(Of Integer)
        For Each c As Char In BankString
            Joltages.Add(CInt(c.ToString()))
        Next
        Return Joltages
    End Function

End Module
