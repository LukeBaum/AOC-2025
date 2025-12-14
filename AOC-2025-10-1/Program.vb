Imports System
Imports System.Data
Imports System.IO

Module Program
    Sub Main(args As String())

        ' I left in an unused first version that appeared to be correct but takes way, waaaaay too long.
        ' Funny what I'd try to do to avoid bitmath...

        Dim Machines As New List(Of Machine)

        For Each Line As String In File.ReadAllLines(args(0))
            Machines.Add(New Machine(Line))
        Next

        Dim TotalButtonPresses As Integer = 0
        For Each M As Machine In Machines
            M.DetermineFewestButtonPressesBinary()
            TotalButtonPresses += M.FewestButtonPresses
        Next

        Console.WriteLine($"Button presses required: {CStr(TotalButtonPresses)}")

    End Sub

    Public Class Machine

        Public OnLightCode As New List(Of Boolean)
        Public Buttons As New List(Of List(Of Boolean))

        Public BinaryOnLightCode As UShort = 0
        Public BinaryButtons As New List(Of UShort)

        Public Property FewestButtonPresses As Integer = Integer.MaxValue

        Sub New(ByVal Line As String)

            Dim Fields() As String = Line.Split(" ")

            For Each Field As String In Fields

                If Field.StartsWith("[") Then
                    Me.ParseLightCode(Field.Substring(1, Field.Length - 2))
                ElseIf Field.StartsWith("(") Then
                    Me.ParseButton(Field.Substring(1, Field.Length - 2))
                End If

            Next

        End Sub

        Private Sub ParseLightCode(ByVal Code As String)
            Me.OnLightCode.Clear()
            For i As Integer = 0 To Code.Length - 1
                Me.OnLightCode.Add(Code.Substring(i, 1) = "#")
            Next
            For i As Integer = Me.OnLightCode.Count - 1 To 0 Step -1
                If Me.OnLightCode(i) = True Then
                    Me.BinaryOnLightCode += 2 ^ CUShort((Me.OnLightCode.Count - 1) - i)
                End If
            Next
        End Sub

        Private Sub ParseButton(ByVal Button As String)
            Dim Action As New List(Of Boolean)
            Dim BAction As UShort = 0
            For i As Integer = 1 To Me.OnLightCode.Count
                Action.Add(False)
            Next
            For Each Position As String In Button.Split(",")
                Action(CInt(Position)) = True
                BAction += 2 ^ CUShort((Me.OnLightCode.Count - 1) - Position)
            Next
            Me.Buttons.Add(Action)
            Me.BinaryButtons.Add(BAction)

        End Sub

        Public Sub Print()

            Dim Output As String = "["
            For Each Code As Boolean In Me.OnLightCode
                If Code = True Then Output &= "#" Else Output &= "."
            Next
            Output &= "] "

            For Each Button As List(Of Boolean) In Me.Buttons
                Dim ButtonString As String = "("
                For i As Integer = 0 To Button.Count - 1
                    If Button(i) = True Then ButtonString &= $"{CStr(i)},"
                Next
                ButtonString = ButtonString.Substring(0, ButtonString.Length - 1)
                Output &= $"{ButtonString}) "
            Next

            Console.WriteLine(Output)

        End Sub

        Public Sub DetermineFewestButtonPressesBinary()

            Dim Sequences As New List(Of (CurrentState As UShort, StepsCompleted As Integer))
            Sequences.Add((0, 0))

            Dim TestedStates As New List(Of UShort)

            While Sequences.Count > 0

                Dim Result As List(Of (CurrentState As UShort, StepsCompleted As Integer, Solved As Boolean)) = GetNextBinary(Sequences(0).CurrentState, Sequences(0).StepsCompleted)
                Sequences.RemoveAt(0)

                For Each Status As (CurrentState As UShort, StepsCompleted As Integer, Solved As Boolean) In Result

                    If Status.Solved Then
                        Me.FewestButtonPresses = Status.StepsCompleted
                        Exit Sub
                    End If

                    'Sequences.Add((Status.CurrentState, Status.StepsCompleted))

                    If Not TestedStates.Contains(Status.CurrentState) Then
                        Sequences.Add((Status.CurrentState, Status.StepsCompleted))
                        TestedStates.Add(Status.CurrentState)
                    End If

                Next

            End While

        End Sub

        Public Sub DetermineFewestButtonPresses()

            Dim CleanSlate As New List(Of Boolean)
            For i As Integer = 1 To Me.OnLightCode.Count
                CleanSlate.Add(False)
            Next

            Dim Sequences As New List(Of (CurrentState As Boolean(), StepsCompleted As Integer, Solved As Boolean))
            Sequences.Add((CleanSlate.ToArray, 0, False))

            Dim TestedStates As New List(Of Boolean())

            Do While Sequences.Count > 0
                Dim Result As List(Of (CurrentState As Boolean(), StepsCompleted As Integer, Solved As Boolean)) = GetNext(Sequences(0).CurrentState, Sequences(0).StepsCompleted)
                Sequences.RemoveAt(0)

                For Each Status As (CurrentState As Boolean(), StepsCompleted As Integer, Solved As Boolean) In Result

                    If Status.Solved Then
                        Me.FewestButtonPresses = Status.StepsCompleted
                        Exit Sub
                    End If

                    If Not TestedStates.Contains(Status.CurrentState) Then
                        Sequences.Add(Status)
                        TestedStates.Add(Status.CurrentState)
                    End If

                Next
            Loop

        End Sub

        Public Function GetNext(ByVal CurrentState() As Boolean, ByVal StepsCompleted As Integer) As List(Of (CurrentState As Boolean(), StepsCompleted As Integer, Solved As Boolean))

            Dim Results As New List(Of (CurrentState As Boolean(), StepsCompleted As Integer, Solved As Boolean))

            StepsCompleted += 1

            For Each Button As List(Of Boolean) In Me.Buttons

                Dim State(CurrentState.Length - 1) As Boolean
                CurrentState.CopyTo(State, 0)

                For i As Integer = 0 To CurrentState.Length - 1
                    If Button(i) = True Then
                        State(i) = Not State(i)
                    End If
                Next

                Dim Solved As Boolean = True
                For i As Integer = 0 To Me.OnLightCode.Count - 1
                    If State(i) <> Me.OnLightCode(i) Then
                        Solved = False
                        Exit For
                    End If
                Next

                Results.Add((State, StepsCompleted, Solved))

            Next

            Return Results

        End Function

        Public Function GetNextBinary(ByVal CurrentState As UShort, ByVal StepsCompleted As Integer) As List(Of (CurrentState As UShort, StepsCompleted As Integer, Solved As Boolean))

            Dim Results As New List(Of (CurrentState As UShort, StepsCompleted As Integer, Solved As Boolean))

            StepsCompleted += 1

            For Each Button As UShort In Me.BinaryButtons
                Dim State As UShort = CurrentState Xor Button
                Results.Add((State, StepsCompleted, (State = Me.BinaryOnLightCode)))
            Next

            Return Results

        End Function

    End Class



End Module
