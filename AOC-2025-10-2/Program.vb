Imports Google.OrTools.LinearSolver

' I did not want to use external tools or libraries but I had to here.
' Also used AI, which I didn't want to do either.

Module ORToolsSolver

    Sub Main(args() As String)

        Dim lines() As String = IO.File.ReadAllLines(args(0))

        Dim grandTotal As Long = 0
        Dim problemCount = 0
        Dim failedCount = 0

        Dim sw = System.Diagnostics.Stopwatch.StartNew()

        For Each line In lines
            problemCount += 1

            Try
                Dim parsed = ParseInput(line)
                Dim buttons = parsed.Item1
                Dim target = parsed.Item2

                Dim solution = SolveButtonPuzzle(buttons, target)

                If solution.Found Then
                    Dim total = solution.Values.Sum()
                    grandTotal += total
                    Console.WriteLine($"{problemCount}: {total} presses ({solution.Method})")
                Else
                    failedCount += 1
                    Console.WriteLine($"{problemCount}: FAILED - {solution.Method}")
                End If
            Catch ex As Exception
                failedCount += 1
                Console.WriteLine($"{problemCount}: ERROR - {ex.Message}")
            End Try
        Next

        sw.Stop()

        Console.WriteLine()
        Console.WriteLine($"Grand Total: {grandTotal}")
        Console.WriteLine($"Problems Solved: {problemCount - failedCount}/{problemCount}")
        Console.WriteLine($"Total Time: {sw.ElapsedMilliseconds}ms")

        If failedCount > 0 Then
            Console.WriteLine($"Failed: {failedCount}")
        End If

        Console.ReadLine()
    End Sub

    Public Class Solution
        Public Property Found As Boolean
        Public Property Values As List(Of Integer)
        Public Property Method As String
    End Class

    ' Parse input format
    Public Function ParseInput(line As String) As (List(Of List(Of Integer)), List(Of Integer))
        Dim buttons As New List(Of List(Of Integer))
        Dim parenStart = line.IndexOf("(")
        Dim braceStart = line.IndexOf("{")

        Dim buttonSection = line.Substring(parenStart, braceStart - parenStart)

        Dim currentPos = 0
        While True
            Dim openParen = buttonSection.IndexOf("(", currentPos)
            If openParen = -1 Then Exit While

            Dim closeParen = buttonSection.IndexOf(")", openParen)
            Dim buttonStr = buttonSection.Substring(openParen + 1, closeParen - openParen - 1)

            Dim positions As New List(Of Integer)
            If buttonStr.Length > 0 Then
                For Each numStr In buttonStr.Split(","c)
                    positions.Add(Integer.Parse(numStr.Trim()))
                Next
            End If
            buttons.Add(positions)

            currentPos = closeParen + 1
        End While

        Dim braceEnd = line.IndexOf("}", braceStart)
        Dim targetStr = line.Substring(braceStart + 1, braceEnd - braceStart - 1)

        Dim target As New List(Of Integer)
        For Each numStr In targetStr.Split(","c)
            target.Add(Integer.Parse(numStr.Trim()))
        Next

        Return (buttons, target)
    End Function

    Public Function SolveButtonPuzzle(buttons As List(Of List(Of Integer)), target As List(Of Integer)) As Solution
        ' Create the solver using SCIP (open source) or CBC
        Dim solver As Solver = Solver.CreateSolver("SCIP")

        If solver Is Nothing Then
            Console.WriteLine("SCIP solver not available, trying CBC...")
            solver = Solver.CreateSolver("CBC")
        End If

        If solver Is Nothing Then
            Return New Solution() With {.Found = False, .Method = "No solver available"}
        End If

        Dim nButtons = buttons.Count
        Dim nPositions = target.Count

        ' Create integer variables for button presses (x[i] >= 0)
        Dim x As New List(Of Variable)
        For i As Integer = 0 To nButtons - 1
            x.Add(solver.MakeIntVar(0, 1000, $"button_{i}"))
        Next

        ' Add constraints: for each position, sum of contributions = target
        For pos As Integer = 0 To nPositions - 1
            Dim constraint = solver.MakeConstraint(target(pos), target(pos), $"position_{pos}")

            For btn As Integer = 0 To nButtons - 1
                If buttons(btn).Contains(pos) Then
                    constraint.SetCoefficient(x(btn), 1)
                End If
            Next
        Next

        ' Objective: minimize total button presses
        Dim objective = solver.Objective()
        For i As Integer = 0 To nButtons - 1
            objective.SetCoefficient(x(i), 1)
        Next
        objective.SetMinimization()

        ' Solve
        Dim resultStatus = solver.Solve()

        If resultStatus = Solver.ResultStatus.OPTIMAL OrElse resultStatus = Solver.ResultStatus.FEASIBLE Then
            Dim solution As New List(Of Integer)
            For i As Integer = 0 To nButtons - 1
                solution.Add(CInt(Math.Round(x(i).SolutionValue())))
            Next

            Return New Solution() With {
                .Found = True,
                .Values = solution,
                .Method = If(resultStatus = Solver.ResultStatus.OPTIMAL, "OR-Tools (optimal)", "OR-Tools (feasible)")
            }
        Else
            Return New Solution() With {
                .Found = False,
                .Method = $"OR-Tools failed: {resultStatus}"
            }
        End If
    End Function

End Module