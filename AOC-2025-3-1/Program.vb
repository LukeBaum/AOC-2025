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

    Private Function FindSumOfMaximumJoltages(ByRef BankStrings() As String) As Integer

        Dim JoltageSum As Integer = 0

        For Each BankString As String In BankStrings
            Dim m As Integer = FindMaximumJoltage(ConvertBankStringToJoltageList(BankString))
            JoltageSum += m
        Next

        Return JoltageSum

    End Function

    Private Function FindMaximumJoltage(ByRef JoltageList As List(Of Integer)) As Integer

        Dim Max As Integer = JoltageList.GetRange(0, JoltageList.Count - 1).Max
        Dim MaxPosition As Integer = JoltageList.IndexOf(Max)
        Dim SecondMax As Integer = JoltageList.GetRange(MaxPosition + 1, JoltageList.Count - (MaxPosition + 1)).Max

        Return CInt(CStr(Max) & CStr(SecondMax))

    End Function

    Private Function ConvertBankStringToJoltageList(ByRef BankString As String) As List(Of Integer)
        Dim Joltages As New List(Of Integer)
        For Each c As Char In BankString
            Joltages.Add(CInt(c.ToString()))
        Next
        Return Joltages
    End Function

End Module
