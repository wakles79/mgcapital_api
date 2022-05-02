export class TagColor {
  name: string;
  hexCode: string;

  constructor(color) {
    this.name = color.name || '';
    this.hexCode = color.hexCode || '';
  }
}

export const TAG_COLORS: TagColor[] = [
  { name: 'Color 1', hexCode: '#000000' },
  { name: 'Color 2', hexCode: '#153C65' },
  { name: 'Color 3', hexCode: '#907F9F' },
  { name: 'Color 4', hexCode: '#339989' },
  { name: 'Color 5', hexCode: '#019547' },
  { name: 'Color 6', hexCode: '#F4D35E' },
  { name: 'Color 7', hexCode: '#FFA686' },
  { name: 'Color 8', hexCode: '#E6E1C5' },
  { name: 'Color 9', hexCode: '#A9BCD0' },
  { name: 'Color 10', hexCode: '#FFFFFF' }
];
