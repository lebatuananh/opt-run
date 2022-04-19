export enum TemplateStatus {
  Draft,
  Publish,
  Archive
}

export class TemplateType {
  static Sms = new TemplateType(1, 'sms', '/assets/images/logo/icon_message.svg');
  static Zalo = new TemplateType(2, 'zalo', '/assets/images/logo/icon_zalo.svg');
  static Email = new TemplateType(3, 'email', '/assets/images/logo/icon_email.png');
  static Viber = new TemplateType(4, 'viber', '/assets/images/logo/icon_viber.png');

  static All = [TemplateType.Sms, TemplateType.Zalo, TemplateType.Email, TemplateType.Viber];

  id: number;
  name: string;
  logo: string;

  constructor(id: number, name: string, logo: string) {
    this.id = id;
    this.name = name;
    this.logo = logo;
  }

  static fromId(id: number): TemplateType {
    const result = TemplateType.All.filter((t) => t.id === id)[0];
    return result;
  }

  static fromName(name: string): TemplateType {
    const result = TemplateType.All.filter(
      (t) => t.name.toLowerCase() === name.toLowerCase()
    )[0];
    return result;
  }
}

export enum TemplateLevelType {
  ZaloOneAction,
  ZaloTwoAction,
  Sms,
  Viber,
  Email
}

export enum Status {
  Active,
  InActive,
  Draft
}

export enum TransactionStatus {
  Pending,
  Completed,
  Error,
  Accept
}

export enum PaymentGateway {
  BankTransfer,
  Momo,
  VnPay,
  ZaloPay,
  Wallet,
}

export enum Action{
  Recharge,
  Deduction,
  Refund
}

export enum MessageStatus {
  NotStarted,
  Processing,
  Success,
  Fail
}

export class MessageType {
  static Sms = new MessageType(1, 'sms', '/assets/images/logo/icon_message.svg');
  static Zalo = new MessageType(2, 'zalo', '/assets/images/logo/icon_zalo.svg');
  static Email = new MessageType(3, 'email', '/assets/images/logo/icon_email.png');
  static Viber = new MessageType(4, 'viber', '/assets/images/logo/icon_viber.png');

  static All = [MessageType.Sms, MessageType.Zalo, MessageType.Email, MessageType.Viber];

  id: number;
  name: string;
  logo: string;

  constructor(id: number, name: string, logo: string) {
    this.id = id;
    this.name = name;
    this.logo = logo;
  }

  static fromId(id: number): MessageType {
    const result = MessageType.All.filter((t) => t.id === id)[0];
    return result;
  }

  static fromName(name: string): MessageType {
    const result = MessageType.All.filter(
      (t) => t.name.toLowerCase() === name.toLowerCase()
    )[0];
    return result;
  }
}
