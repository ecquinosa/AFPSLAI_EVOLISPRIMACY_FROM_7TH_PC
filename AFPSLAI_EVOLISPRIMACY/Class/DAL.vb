
Imports System.Data.SqlClient

Public Class DAL
    Implements IDisposable
    
    'Private ConStr As String = "Server=192.168.1.114\ACCDEV;Database=dbcAFPSLAI;User=sa;Password=acc2016;"
    Private Shared ConStr As String = "Server=" & My.Settings.Server & ";Database=" & My.Settings.Database & ";User=" & My.Settings.User & ";Password=" & My.Settings.Password & ";"

    Private dtResult As DataTable
    Private dsResult As DataSet
    Private objResult As Object
    Private _readerResult As IDataReader
    Private strErrorMessage As String

    Private con As SqlConnection
    Private cmd As SqlCommand
    Private da As SqlDataAdapter
    Private _UserID As String

    Public Sub New()
    End Sub

    Public Sub New(ByVal _UserID As String)
        Me._UserID = _UserID
    End Sub

    Public ReadOnly Property ErrorMessage() As String
        Get
            Return strErrorMessage
        End Get
    End Property

    Public ReadOnly Property TableResult() As DataTable
        Get
            Return dtResult
        End Get
    End Property

    Public ReadOnly Property DatasetResult() As DataSet
        Get
            Return dsResult
        End Get
    End Property

    Public ReadOnly Property ObjectResult() As Object
        Get
            Return objResult
        End Get
    End Property

    Public ReadOnly Property ReaderResult() As IDataReader
        Get
            Return _readerResult
        End Get
    End Property

    Public Sub ClearAllPools()
        SqlConnection.ClearAllPools()
    End Sub

    Private Sub OpenConnection()
        If con Is Nothing Then con = New SqlConnection(ConStr)
    End Sub

    Private Sub CloseConnection()
        If Not cmd Is Nothing Then cmd.Dispose()
        If Not da Is Nothing Then da.Dispose()
        If Not _readerResult Is Nothing Then
            _readerResult.Close()
            _readerResult.Dispose()
        End If
        If Not con Is Nothing Then If con.State = ConnectionState.Open Then con.Close()
        ClearAllPools()
    End Sub

    Private Sub ExecuteNonQuery(ByVal cmdType As CommandType)
        cmd.CommandType = cmdType

        'If con.State = ConnectionState.Open Then con.Close()
        'con.Open()
        If con.State = ConnectionState.Closed Then con.Open()
        cmd.ExecuteNonQuery()
        con.Close()
    End Sub

    Private Sub _ExecuteScalar(ByVal cmdType As CommandType)
        cmd.CommandType = cmdType

        'If con.State = ConnectionState.Open Then con.Close()
        'con.Open()
        If con.State = ConnectionState.Closed Then con.Open()
        Dim _obj As Object
        _obj = cmd.ExecuteScalar()
        con.Close()

        objResult = _obj
    End Sub

    Private Sub _ExecuteReader(ByVal cmdType As CommandType)
        cmd.CommandType = cmdType

        'If con.State = ConnectionState.Open Then con.Close()
        'con.Open()
        If con.State = ConnectionState.Closed Then con.Open()
        Dim reader As SqlDataReader = cmd.ExecuteReader

        _readerResult = reader
    End Sub

    Private Sub FillDataAdapter(ByVal cmdType As CommandType)
        cmd.CommandTimeout = 0
        cmd.CommandType = cmdType
        da = New SqlDataAdapter(cmd)
        Dim _dt As New DataTable
        da.Fill(_dt)
        dtResult = _dt
    End Sub

    Public Function IsConnectionOK(Optional ByVal strConString As String = "") As Boolean
        Try
            If strConString <> "" Then ConStr = strConString
            OpenConnection()

            con.Open()
            con.Close()

            Return True
        Catch ex As Exception
            strErrorMessage = ex.Message
            Return False
        End Try
    End Function

    Public Function ExecuteQuery(ByVal strQuery As String) As Boolean
        Try
            OpenConnection()
            cmd = New SqlCommand(strQuery, con)

            ExecuteNonQuery(CommandType.Text)

            Return True
        Catch ex As Exception
            strErrorMessage = ex.Message
            Return False
        End Try
    End Function

    Public Function AddPrinterCounter(ByVal CIF As String, ByVal Before As Integer, ByVal After As Integer) As Boolean
        Try
            OpenConnection()
            cmd = New SqlCommand("prcAddPrinterCounter", con)
            cmd.Parameters.AddWithValue("CIF", CIF)
            cmd.Parameters.AddWithValue("Before", Before)
            cmd.Parameters.AddWithValue("After", After)

            ExecuteNonQuery(CommandType.StoredProcedure)

            Return True
        Catch ex As Exception
            strErrorMessage = ex.Message
            Return False
        End Try
    End Function

    Public Function SelectQuery(ByVal strQuery As String) As Boolean
        Try
            OpenConnection()
            cmd = New SqlCommand(strQuery, con)

            FillDataAdapter(CommandType.Text)

            Return True
        Catch ex As Exception
            strErrorMessage = ex.Message
            Return False
        End Try
    End Function

    Public Function InsertRelDataCardActivity(ByVal CIF As String, ByVal Activity As String) As Boolean
        Try
            OpenConnection()
            cmd = New SqlCommand(String.Format("INSERT INTO tblRelDataCardActivity (CIF, Activity, DatePosted) VALUES ('{0}','{1}', GETDATE())", CIF, Activity), con)

            ExecuteNonQuery(CommandType.Text)

            Return True
        Catch ex As Exception
            strErrorMessage = ex.Message
            Return False
        End Try
    End Function

    Public Function SelectDataByMaxDataID() As Boolean
        Try
            OpenConnection()
            cmd = New SqlCommand("prcSelectDataByMaxDataID", con)

            FillDataAdapter(CommandType.StoredProcedure)

            Return True
        Catch ex As Exception
            strErrorMessage = ex.Message
            Return False
        End Try
    End Function

    Public Function SelectDataByCIF(ByVal CIF As String) As Boolean
        Try
            OpenConnection()
            cmd = New SqlCommand("prcSelectDataByCIF", con)
            cmd.Parameters.AddWithValue("CIF", CIF)

            FillDataAdapter(CommandType.StoredProcedure)

            Return True
        Catch ex As Exception
            strErrorMessage = ex.Message
            Return False
        End Try
    End Function

    Public Function SelectPrintingTypeSummary(ByVal StartDate As String, ByVal EndDate As String) As Boolean
        Try
            OpenConnection()
            cmd = New SqlCommand("prcSelectPrintingTypeSummary", con)
            cmd.Parameters.AddWithValue("StartDate", StartDate)
            cmd.Parameters.AddWithValue("EndDate", EndDate)

            FillDataAdapter(CommandType.StoredProcedure)

            Return True
        Catch ex As Exception
            strErrorMessage = ex.Message
            Return False
        End Try
    End Function

    Public Function SelectDataByDataTypeIDAndRange(ByVal DataTypeID As Short, ByVal StartDate As Date, ByVal EndDate As Date) As Boolean
        Try
            OpenConnection()
            cmd = New SqlCommand("prcSelectDataByDataTypeIDAndRange", con)
            cmd.Parameters.AddWithValue("@DataTypeID", DataTypeID)
            cmd.Parameters.AddWithValue("@StartDate", StartDate)
            cmd.Parameters.AddWithValue("@EndDate", EndDate)

            FillDataAdapter(CommandType.StoredProcedure)

            Return True
        Catch ex As Exception
            strErrorMessage = ex.Message
            Return False
        End Try
    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
            CloseConnection()

        End If
        Me.disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
