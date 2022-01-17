## Senior C# Developer

�������� [ASP.NET](http://ASP.NET) ���������� ��� �������� �������� �����������.

���������� ������ ������������� ��������� API:

- ����� ��� �������� � ������������� �������� �����������, ������� ������ ��������� ��� � ������ ��� � ������������� ��������� (�� ��� �����). ����� ������ ���������� ������������� ����������� � ������ ��� ��������. �.�. ��� ����� ���� �������� �������, �� �������� �������������� � ������ ������ ����� �� ������. ������� ���������� ������, retry-��������, ������� � �.�. ������������ �� �����.
- ����� ��� ��������� ������� �������� ����������� (����������/�� ����������) �� ��� ��������������.

�� ������ ������, ����������� ����� ���� ���� �����:

1. ��� iOS-���������.
2. ��� Android-���������.

��� iOS ��������� ����������� ������������ �� ����:

- PushToken - ������ �� 50 ��������. ���������� ������������� �������, ���� ����� ���������� �����������. ���� ������������.
- Alert - ������ �� 2000 ��������. ���� ���������. ���� ������������.
- Priority - ����� �����. �� ��������� ������ ��������� �������� 10.
- IsBackground - ������ ��������. �� ��������� ������ ��������� �������� true.

��� Android ��������� ����������� ������������ �� ����:

- DeviceToken - ������ �� 50 ��������. ���������� ������������� �������, ���� ����� ���������� �����������. ���� ������������.
- Message - ������ �� 2000 ��������. ���� ���������. ���� ������������.
- Title - ������ �� 255 ��������. ���� ������������.
- Condition - ������ �� 2000 ��������. ���� �������� ������������.

� ����������� �� ���� ����������� ������ ���� ������ ������������ NotificationSender (������, ������� ���������� ���������� �����������).

��� NotificationSender ����� ���� ���������� ����������� ������� � �������������� Task.Delay � Logger (� ��� ���������� ������� user-friendly ��� �������� NotificationSender � ���� �����������). 

����� ����������� �������� (delay) ������ ������������� �� 500�� �� 2 ������ �� ������� � �������.

� ������ ������� NotificationSender: ������ ����� �������� ������ ����������� ��������� (������ ����������� ����� ��� ����������). � ������ �������� �������� ������ ����������� ����� �����������.

���������� ������, ��� ����� �������� � �������� ����������� ����� �������� ����� ����� �� ����� ������ �������������. 

������� ������� � �������:

- Unit-�����
- ���������� �������� � ���������� (��������� ������ �������� ��������� ������ ���� �����������, ��������, �� Email)

���������� ������ ����������� ��� �������������� ������������ ��� ��������� ��������������� ��.

��� ����: .NET 5 (� ����) + PostgreSQL + Docker.

��� ��������� ������� ����� � ������ ����� �������, �� ��������������� ����������� ����������. �������� ������� � ��������� ������� � readme.md