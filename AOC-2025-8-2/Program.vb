' Claude made this one, too...

Imports System.IO
Imports System.Math

Module Program
    Class UnionFind
        Private parent As Integer()
        Private size As Integer()
        Private componentCount As Integer

        Public Sub New(n As Integer)
            parent = New Integer(n - 1) {}
            size = New Integer(n - 1) {}
            componentCount = n
            For i As Integer = 0 To n - 1
                parent(i) = i
                size(i) = 1
            Next
        End Sub

        Public Function Find(x As Integer) As Integer
            If parent(x) <> x Then
                parent(x) = Find(parent(x))
            End If
            Return parent(x)
        End Function

        Public Function Union(x As Integer, y As Integer) As Boolean
            Dim rootX = Find(x)
            Dim rootY = Find(y)

            If rootX = rootY Then
                Return False
            End If

            If size(rootX) < size(rootY) Then
                parent(rootX) = rootY
                size(rootY) += size(rootX)
            Else
                parent(rootY) = rootX
                size(rootX) += size(rootY)
            End If

            componentCount -= 1
            Return True
        End Function

        Public Function GetComponentCount() As Integer
            Return componentCount
        End Function
    End Class

    Structure Point3D
        Public X As Integer
        Public Y As Integer
        Public Z As Integer

        Public Sub New(x As Integer, y As Integer, z As Integer)
            Me.X = x
            Me.Y = y
            Me.Z = z
        End Sub

        Public Function DistanceSquared(other As Point3D) As Long
            Dim dx As Long = CLng(Me.X) - CLng(other.X)
            Dim dy As Long = CLng(Me.Y) - CLng(other.Y)
            Dim dz As Long = CLng(Me.Z) - CLng(other.Z)
            Return dx * dx + dy * dy + dz * dz
        End Function
    End Structure

    Structure Edge
        Public I As Integer
        Public J As Integer
        Public DistSq As Long

        Public Sub New(i As Integer, j As Integer, distSq As Long)
            Me.I = i
            Me.J = j
            Me.DistSq = distSq
        End Sub
    End Structure

    Sub Main(args As String())
        If args.Length = 0 Then
            Console.WriteLine("Usage: program <input_file_path>")
            Return
        End If

        Dim points As New List(Of Point3D)

        For Each line In File.ReadAllLines(args(0))
            Dim parts = line.Split(","c)
            If parts.Length = 3 Then
                points.Add(New Point3D(Integer.Parse(parts(0)),
                                      Integer.Parse(parts(1)),
                                      Integer.Parse(parts(2))))
            End If
        Next

        Dim edges As New List(Of Edge)

        For i As Integer = 0 To points.Count - 1
            For j As Integer = i + 1 To points.Count - 1
                Dim distSq = points(i).DistanceSquared(points(j))
                edges.Add(New Edge(i, j, distSq))
            Next
        Next

        edges.Sort(Function(a, b) a.DistSq.CompareTo(b.DistSq))

        Dim uf As New UnionFind(points.Count)
        Dim lastI As Integer = -1
        Dim lastJ As Integer = -1

        For Each edge In edges
            If uf.Union(edge.I, edge.J) Then
                lastI = edge.I
                lastJ = edge.J

                If uf.GetComponentCount() = 1 Then
                    Exit For
                End If
            End If
        Next

        Dim result As Long = CLng(points(lastI).X) * CLng(points(lastJ).X)
        Console.WriteLine(result)
    End Sub
End Module