Imports System
Imports System.IO
Imports System.Collections.Generic

Module PuzzleSolver
    Sub Main(args As String())
        ' Check if file path argument is provided
        If args.Length = 0 Then
            Console.WriteLine("Error: Please provide a file path as an argument")
            Environment.Exit(1)
        End If

        Dim filePath As String = args(0)

        ' Check if file exists
        If Not File.Exists(filePath) Then
            Console.WriteLine("Error: File not found - " & filePath)
            Environment.Exit(1)
        End If

        ' Read all lines from the file
        Dim lines As String() = File.ReadAllLines(filePath)

        If lines.Length = 0 Then
            Console.WriteLine("Error: File is empty")
            Environment.Exit(1)
        End If

        ' Convert to a mutable 2D character array
        Dim grid As Char()() = New Char(lines.Length - 1)() {}
        For i As Integer = 0 To lines.Length - 1
            grid(i) = lines(i).ToCharArray()
        Next

        ' Find the starting position (S on the top line)
        Dim startCol As Integer = -1
        For col As Integer = 0 To grid(0).Length - 1
            If grid(0)(col) = "S"c Then
                startCol = col
                Exit For
            End If
        Next

        If startCol = -1 Then
            Console.WriteLine("Error: No 'S' found on the first line")
            Environment.Exit(1)
        End If

        ' Solve the puzzle using a queue for breadth-first traversal
        Dim caretCount As Integer = 0
        Dim queue As New Queue(Of Tuple(Of Integer, Integer))()
        Dim visited As New HashSet(Of Tuple(Of Integer, Integer))()

        ' Start from the position below S
        If grid.Length > 1 Then
            queue.Enqueue(New Tuple(Of Integer, Integer)(1, startCol))
        End If

        While queue.Count > 0
            Dim current As Tuple(Of Integer, Integer) = queue.Dequeue()
            Dim row As Integer = current.Item1
            Dim col As Integer = current.Item2

            ' Check bounds
            If row < 0 OrElse row >= grid.Length OrElse col < 0 OrElse col >= grid(row).Length Then
                Continue While
            End If

            ' Skip if already visited
            If visited.Contains(current) Then
                Continue While
            End If
            visited.Add(current)

            Dim currentChar As Char = grid(row)(col)

            If currentChar = "."c Then
                ' Replace period with pipe and continue downward
                grid(row)(col) = "|"c
                If row + 1 < grid.Length Then
                    queue.Enqueue(New Tuple(Of Integer, Integer)(row + 1, col))
                End If

            ElseIf currentChar = "^"c Then
                ' Encountered a caret - count it
                caretCount += 1

                ' Branch left and right on the same row
                ' Left
                If col - 1 >= 0 Then
                    queue.Enqueue(New Tuple(Of Integer, Integer)(row, col - 1))
                End If
                ' Right
                If col + 1 < grid(row).Length Then
                    queue.Enqueue(New Tuple(Of Integer, Integer)(row, col + 1))
                End If
            End If
        End While

        ' Output the answer
        Console.WriteLine(caretCount)
    End Sub
End Module