Imports System
Imports System.IO
Imports System.Collections.Generic
Imports System.Linq

Module Program
    Sub Main(args() As String)

        Dim lines() As String = File.ReadAllLines(args(0))

        Dim redTiles As New List(Of Tuple(Of Integer, Integer))()

        For Each line As String In lines
            If Not String.IsNullOrWhiteSpace(line) Then
                Dim parts() As String = line.Split(","c)
                Dim x As Integer = Integer.Parse(parts(0).Trim())
                Dim y As Integer = Integer.Parse(parts(1).Trim())
                redTiles.Add(New Tuple(Of Integer, Integer)(x, y))
            End If
        Next

        Dim boundaryTiles As New HashSet(Of Tuple(Of Integer, Integer))()

        For Each tile In redTiles
            boundaryTiles.Add(tile)
        Next

        For i As Integer = 0 To redTiles.Count - 1
            Dim current = redTiles(i)
            Dim nextTile = redTiles((i + 1) Mod redTiles.Count)

            If current.Item1 = nextTile.Item1 Then ' Vertical line
                Dim minY As Integer = Math.Min(current.Item2, nextTile.Item2)
                Dim maxY As Integer = Math.Max(current.Item2, nextTile.Item2)
                For y As Integer = minY To maxY
                    boundaryTiles.Add(New Tuple(Of Integer, Integer)(current.Item1, y))
                Next
            ElseIf current.Item2 = nextTile.Item2 Then ' Horizontal line
                Dim minX As Integer = Math.Min(current.Item1, nextTile.Item1)
                Dim maxX As Integer = Math.Max(current.Item1, nextTile.Item1)
                For x As Integer = minX To maxX
                    boundaryTiles.Add(New Tuple(Of Integer, Integer)(x, current.Item2))
                Next
            End If
        Next

        Console.WriteLine("Boundary tiles: " & boundaryTiles.Count)
        Dim maxArea As Long = 0
        Dim bestI As Integer = -1
        Dim bestJ As Integer = -1
        Dim count As Integer = 0

        Console.WriteLine(vbCrLf & "Searching for largest rectangle...")
        For i As Integer = 0 To redTiles.Count - 1
            If i Mod 50 = 0 Then
                Console.WriteLine("Progress: " & i & "/" & redTiles.Count & " tiles (current best: " & maxArea & ")...")
            End If

            For j As Integer = i + 1 To redTiles.Count - 1
                count += 1
                Dim tile1 = redTiles(i)
                Dim tile2 = redTiles(j)

                Dim rectMinX As Integer = Math.Min(tile1.Item1, tile2.Item1)
                Dim rectMaxX As Integer = Math.Max(tile1.Item1, tile2.Item1)
                Dim rectMinY As Integer = Math.Min(tile1.Item2, tile2.Item2)
                Dim rectMaxY As Integer = Math.Max(tile1.Item2, tile2.Item2)

                Dim width As Long = CLng(rectMaxX - rectMinX + 1)
                Dim height As Long = CLng(rectMaxY - rectMinY + 1)
                Dim area As Long = width * height

                If area <= maxArea Then
                    Continue For
                End If

                If IsRectangleValidSampled(rectMinX, rectMaxX, rectMinY, rectMaxY, boundaryTiles, redTiles) Then
                    If area > maxArea Then
                        maxArea = area
                        bestI = i
                        bestJ = j
                        Console.WriteLine("  New best: " & area & " from " & tile1.ToString() & " to " & tile2.ToString())
                    End If
                End If
            Next
        Next

        Console.WriteLine(vbCrLf & "Checked " & count & " pairs")

        If bestI >= 0 Then
            Console.WriteLine(vbCrLf & "Best rectangle:")
            Console.WriteLine("  From: " & redTiles(bestI).ToString())
            Console.WriteLine("  To: " & redTiles(bestJ).ToString())
            Dim w As Long = Math.Abs(redTiles(bestJ).Item1 - redTiles(bestI).Item1) + 1
            Dim h As Long = Math.Abs(redTiles(bestJ).Item2 - redTiles(bestI).Item2) + 1
            Console.WriteLine("  Dimensions: " & w & " x " & h)
        End If

        Console.WriteLine(vbCrLf & "Part 2: " & maxArea)
    End Sub

    Function IsTileValid(x As Integer, y As Integer, boundaryTiles As HashSet(Of Tuple(Of Integer, Integer)), polygon As List(Of Tuple(Of Integer, Integer))) As Boolean
        If boundaryTiles.Contains(New Tuple(Of Integer, Integer)(x, y)) Then
            Return True
        End If
        Return IsInsidePolygon(x, y, polygon)
    End Function

    Function IsRectangleValidSampled(minX As Integer, maxX As Integer, minY As Integer, maxY As Integer, boundaryTiles As HashSet(Of Tuple(Of Integer, Integer)), polygon As List(Of Tuple(Of Integer, Integer))) As Boolean
        Dim width As Long = CLng(maxX - minX + 1)
        Dim height As Long = CLng(maxY - minY + 1)

        If width * height < 10000 Then
            For x As Integer = minX To maxX
                For y As Integer = minY To maxY
                    If Not IsTileValid(x, y, boundaryTiles, polygon) Then
                        Return False
                    End If
                Next
            Next
            Return True
        End If

        For Each x In {minX, maxX}
            For Each y In {minY, maxY}
                If Not IsTileValid(x, y, boundaryTiles, polygon) Then
                    Return False
                End If
            Next
        Next

        Dim stepSize As Integer = 100
        For x As Integer = minX To maxX Step stepSize
            If Not IsTileValid(x, minY, boundaryTiles, polygon) OrElse Not IsTileValid(x, maxY, boundaryTiles, polygon) Then
                Return False
            End If
        Next
        For y As Integer = minY To maxY Step stepSize
            If Not IsTileValid(minX, y, boundaryTiles, polygon) OrElse Not IsTileValid(maxX, y, boundaryTiles, polygon) Then
                Return False
            End If
        Next

        stepSize = 500
        For x As Integer = minX + stepSize To maxX - 1 Step stepSize
            For y As Integer = minY + stepSize To maxY - 1 Step stepSize
                If Not IsTileValid(x, y, boundaryTiles, polygon) Then
                    Return False
                End If
            Next
        Next

        Return True
    End Function

    Function IsInsidePolygon(x As Integer, y As Integer, polygon As List(Of Tuple(Of Integer, Integer))) As Boolean
        Dim inside As Boolean = False
        Dim n As Integer = polygon.Count

        For i As Integer = 0 To n - 1
            Dim p1 = polygon(i)
            Dim p2 = polygon((i + 1) Mod n)

            Dim x1 As Integer = p1.Item1
            Dim y1 As Integer = p1.Item2
            Dim x2 As Integer = p2.Item1
            Dim y2 As Integer = p2.Item2

            If ((y1 > y) <> (y2 > y)) Then
                Dim slope As Double = CDbl(x2 - x1) / CDbl(y2 - y1)
                Dim intersectX As Double = x1 + slope * (y - y1)
                If x < intersectX Then
                    inside = Not inside
                End If
            End If
        Next

        Return inside
    End Function

End Module